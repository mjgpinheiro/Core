from __future__ import print_function
from builtins import *
from scapy.all import *
import datetime
import sys
import json
import os
import time
from joblib import Parallel, delayed

import multiprocessing
from subprocess import call
import shutil
import uuid

'''
	QUANTLER.COM - Quant Fund Development Platform
	Quantler Core Trading Engine. Copyright 2018 Quantler B.V.

	Licensed under the Apache License, Version 2.0 (the "License"); 
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.

	This script defines a custom binary protocol :
	- IEX transport protocol implementing https://iextrading.com/docs/IEX%20Transport%20Specification.pdf
	- IEX-TOPS 1.5 messages spec implementing  https://iextrading.com/docs/IEX%20TOPS%20Specification%20v1.5.pdf

	Protocol is the used to read input file in pcap format and output IEX-TOPS1.5 messages in json format 

	Usage : 

	iexTopsPcap2Json.1.15.py <inputfile.pcap> <outputfile.json>
'''


#IEXTP message types
T = 0x54
B = 0x42
Q = 0x51


#This dict describes for each message type, the value flag and it's hex value such as flag = 1 if hexValue & messageFlag
FLAGS = { 
	Q : 
		{
		'H' : 0x80,
		'P' : 0x40
		},
	T	: {
		'F' : 0x80,
		'T' : 0x40,
		'I' : 0x20,
		'B' : 0x20
		},
	B	: {
		'F' : 0x80,
		'T' : 0x40,
		'I' : 0x20,
		'B' : 0x20
		}
}

class IEXTP_Message(Packet) :	
	name = "IEX-TP-MESSAGE"

	#Message structure
	fields_desc = [
		LEShortField("messageLength",0),
		XByteField("messageType",0),
		XByteField("flagNames",0),
		LELongField("timestamp",0),
		StrFixedLenField("symbol","",length=8),
		#Q type messages
		ConditionalField(LEIntField("bidSize",0),lambda pkt:pkt.messageType==Q),
		ConditionalField(LELongField("bidPrice",0),lambda pkt:pkt.messageType==Q),
		ConditionalField(LELongField("askPrice",0),lambda pkt:pkt.messageType==Q),
		ConditionalField(LEIntField("askSize",0),lambda pkt:pkt.messageType==Q),

		#T/B type messages
		ConditionalField(LEIntField("size",0),lambda pkt:pkt.messageType in (T,B)),
		ConditionalField(LELongField("price",0),lambda pkt:pkt.messageType in (T,B)),
		ConditionalField(LELongField("tradeId",0),lambda pkt:pkt.messageType in (T,B)),
		ConditionalField(LEIntField("reserved",0),lambda pkt:pkt.messageType in (T,B)),
	]
	
	#data formating
	def toDict(self) :

		messageFlag = {}
		for flagName in FLAGS[self.messageType] :
			messageFlag[flagName] = 1 if FLAGS[self.messageType][flagName]&self.flagNames else 0

		data = {
				'flags' : messageFlag,
				'messageType':chr(self.messageType),
				"timestamp" : str(datetime.datetime.utcfromtimestamp(self.timestamp/1000000000.0)),
				"symbol" : self.symbol.decode('utf-8').strip()
			}

		if self.messageType == Q : 
			data['bidSize'] = self.bidSize
			data['bidPrice'] = self.bidPrice
			data['askPrice'] = self.askPrice
			data['askSize'] = self.askSize
		elif self.messageType in (T,B) :
			data['size'] = self.size
			data['price'] = self.price
			data['tradeId'] = self.tradeId
		else : 
			raise("Unknown message type",self.messageType)
		return data


	#https://stackoverflow.com/questions/8073508/scapy-adding-new-protocol-with-complex-field-groupings
	def extract_padding(self, s):
		return '', s

class IEXTP(Packet) : 

	name = "IEX-TP"
	fields_desc=[ 
		XByteField("version",0),
		XByteField("reserved",0) ,
		LEShortField("messageProtocolId",0),
		LEIntField("channelId",0),
		LEIntField("sessionId",0),
		LEShortField("payloadLength",0),
		LEShortField("messageCount",0),
		LELongField("streamOffset",0),
		LELongField("firstMessageSequenceNumber",0),
		LELongField("timeStamp",0),
		PacketListField("messages",None,IEXTP_Message,count_from = lambda pkt: pkt.messageCount )
		]

	def toDict(self) :
		return {
				"messageProtocolId" : hex(self.messageProtocolId),
				"sessionId" : self.sessionId,
				"messageCount" : self.messageCount,
				"messages" : [msg.toDict() for msg in self.messages]
			}
	
#IEXTP can be over UDP as well as TCP
bind_layers( UDP, IEXTP )
bind_layers( TCP, IEXTP )

def worker(inputPcapFilename,outputJsonFilename,i) :

	inputFileSize = os.stat(inputPcapFilename).st_size
	processedBytesCounter = 0
	with PcapReader(inputPcapFilename) as pcap_reader,open(outputJsonFilename,'wb') as outputfile:
			
			for pkNum,pkt in enumerate(pcap_reader):
				#print(pkNum)
				processedBytesCounter+=len(pkt)
				if not pkNum%10000 :
					print("worker",i,":","~",round(100.0*processedBytesCounter/inputFileSize,2),"%")
				
				segment = pkt[IEXTP]
				outputfile.write(json.dumps(segment.toDict())+"\n")

def main() : 
	#check cli arguments
	if len(sys.argv)<3 : 
		print("Usage : python %s <inputfile.pcap> <outputfile.json>"%sys.argv[0])
		sys.exit(1)

	nbCpu = multiprocessing.cpu_count()

	inputPcapFilename = sys.argv[1]
	outputJsonFilename = sys.argv[2]
	
	inputFileSize = os.stat(inputPcapFilename).st_size
	splitSize = (inputFileSize/nbCpu/1000.0/1000.0)+1

	workspace = str(uuid.uuid1())
	try :

		print("creating workspace %s"%workspace)
		os.mkdir(workspace)

		print("splitting %s into %d pieces of %dMB"%(inputPcapFilename,nbCpu,splitSize)) 
		call(["tcpdump", "-r",inputPcapFilename,"-w",workspace+"/split","-C",str(splitSize)])

		print("forking %d parallel processes for parsing"%nbCpu)

		jobs = enumerate(os.listdir(workspace))

		Parallel(n_jobs=nbCpu, backend="multiprocessing")(delayed(worker)(workspace+"/"+filename,workspace+"/"+filename+".json",i) 
					for i,filename in jobs)

		print("parsing done, rebuilding output json")
		with open(outputJsonFilename,'wb') as bigJson :
			for smallerJsonFilename in sorted([workspace+"/"+filename for filename in os.listdir(workspace) if ".json" in filename]) :
				with open(smallerJsonFilename) as smallerJson :
					for line in smallerJson :
						bigJson.write(line)
		print("all done!")
	finally : 
		print ("deleting %s"%workspace)
		shutil.rmtree(workspace)

if __name__ == '__main__':
	main()
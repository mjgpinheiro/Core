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
	- IEX-TOPS 1.6 messages spec implementing  https://iextrading.com/docs/IEX%20TOPS%20Specification.pdf

	Protocol is the used to read input file in pcap format and output IEX-TOPS1.6 messages in json format 

	Usage : 

	iexTopsPcap2Json.1.16.py <inputfile.pcap> <outputfile.json>
'''


#IEXTP message types
T = 0x54
B = 0x42
Q = 0x51

#New TOPS 1.6 message types
S = 0x53
D = 0x44
H = 0x48
O = 0x4f
P = 0x50
X = 0x58
A = 0x41

#This dict describes for each message type, the value flag and it's hex value such as flag = 1 if hexValue & messageFlag
FLAGS = { 

    D : {
    	'T' :0x80,
		'W' :0x40,
    	'E' :0x20
	    },
	Q : 
		{
		'A' : 0x80,
		'P' : 0x40
		},
	T	: {
		'F' : 0x80,
		'T' : 0x40,
		'I' : 0x20,
		'8' : 0x10,
		'X' : 0x08
		},
	B	: {
		'F' : 0x80,
		'T' : 0x40,
		'I' : 0x20,
		'8' : 0x10,
		'X' : 0x08
		}
}

#field description/
MESSAGE_LEN = LEShortField("messageLength",0)
MESSAGE_TYPE = XByteField("messageType",0)
SYSTEN_EVENT = XByteField("systemEvent",0)
FLAG_NAMES = XByteField("flagNames",0)
TIMESTAMP = LELongField("timestamp",0)
SYMBOL = StrFixedLenField("symbol","",length=8)
BID_SIZE = LEIntField("bidSize",0)
BID_PRICE = LELongField("bidPrice",0)
ASK_PRICE = LELongField("askPrice",0)
ASK_SIZE = LEIntField("askSize",0)
SIZE = LEIntField("size",0)
PRICE = LELongField("price",0)
TRADE_ID = LELongField("tradeId",0)
RESERVED = LEIntField("reserved",0)
ROUND_LOT_SIZE = LEIntField("roundLotSize",0)
ADJUSTED_POC_PRICE = LELongField("adjustedPocPrice",0)
LULD_TIER = XByteField("LULDTier",0)
TRADING_STATUS = XByteField("tradingStatus",0)
REASON = StrFixedLenField("reason","",length=4)
OPERATIONAL_HALT_STATUS = XByteField("operationalHaltStatus",0)
SHORT_SALE_PRICE_TEST_STATUS = XByteField("shortSalePriceTestStatus",0)
DETAIL = XByteField("detail",0)
AUCTION_TYPE = XByteField("auctionType",0)
PAIRED_SHARES = LEIntField("pairedShares",0)
REFERENCE_PRICE = LELongField("referencePrice",0)
INDICATIVE_CLEARING_PRICE = LELongField("indicativeClearingPrice",0)
IMBALANCE_SHARES = LEIntField("imbalanceShares",0)
IMBALANCE_SIDE = XByteField("imbalanceSide",0)
EXTENSION_NUMBER = ByteField("extensionNumber",0)
SCHEDULED_AUCTION_TIME = LEIntField("scheduledAuctionTime",0)
AUCTION_BOOK_CLEARING_PRICE = LELongField("auctionBookClearingPrice",0)
COLLAR_REFERENCE_PRICE = LELongField("collarReferencePrice",0)
LOWER_AUCTION_COLLAR = LELongField("lowerAuctionCollar",0)
UPPER_AUCTION_COLLAR = LELongField("upperAuctionCollar",0)
PRICE_TYPE = XByteField("priceType",0)
OFFICIAL_PRICE = LELongField("officialPrice",0)


class IEXTP_Message(Packet) :	
	name = "IEX-TP-MESSAGE"

	#Message structure
	fields_desc = [
			MESSAGE_LEN,
			MESSAGE_TYPE,
			ConditionalField(SYSTEN_EVENT,lambda pkt:pkt.messageType==S),
			ConditionalField(FLAG_NAMES,lambda pkt:pkt.messageType in [D,Q,T,B]),
			ConditionalField(TRADING_STATUS,lambda pkt:pkt.messageType==H),
			ConditionalField(OPERATIONAL_HALT_STATUS,lambda pkt:pkt.messageType==O),
			ConditionalField(SHORT_SALE_PRICE_TEST_STATUS,lambda pkt:pkt.messageType==P),
			ConditionalField(PRICE_TYPE,lambda pkt:pkt.messageType==X),
			ConditionalField(AUCTION_TYPE,lambda pkt:pkt.messageType==A),

			TIMESTAMP,
			ConditionalField(SYMBOL,lambda pkt:pkt.messageType!=S),
			
			ConditionalField(ROUND_LOT_SIZE,lambda pkt:pkt.messageType==D),
			ConditionalField(ADJUSTED_POC_PRICE,lambda pkt:pkt.messageType==D),
			ConditionalField(LULD_TIER,lambda pkt:pkt.messageType==D),


			ConditionalField(REASON,lambda pkt:pkt.messageType==H),

			ConditionalField(DETAIL,lambda pkt:pkt.messageType==P),

			ConditionalField(BID_SIZE,lambda pkt:pkt.messageType==Q),
			ConditionalField(BID_PRICE,lambda pkt:pkt.messageType==Q),
			ConditionalField(ASK_PRICE,lambda pkt:pkt.messageType==Q),
			ConditionalField(ASK_SIZE,lambda pkt:pkt.messageType==Q),

			ConditionalField(SIZE,lambda pkt:pkt.messageType in [T,B]),
			ConditionalField(PRICE,lambda pkt:pkt.messageType in [T,B]),
			ConditionalField(TRADE_ID,lambda pkt:pkt.messageType in [T,B]),


			ConditionalField(BID_SIZE,lambda pkt:pkt.messageType==X),
			ConditionalField(BID_PRICE,lambda pkt:pkt.messageType==X),
			ConditionalField(ASK_PRICE,lambda pkt:pkt.messageType==X),
			ConditionalField(ASK_SIZE,lambda pkt:pkt.messageType==X),

			ConditionalField(PAIRED_SHARES,lambda pkt:pkt.messageType==A),
			ConditionalField(REFERENCE_PRICE,lambda pkt:pkt.messageType==A),
			ConditionalField(INDICATIVE_CLEARING_PRICE,lambda pkt:pkt.messageType==A),
			ConditionalField(IMBALANCE_SHARES,lambda pkt:pkt.messageType==A),
			ConditionalField(IMBALANCE_SIDE,lambda pkt:pkt.messageType==A),
			ConditionalField(EXTENSION_NUMBER,lambda pkt:pkt.messageType==A),
			ConditionalField(SCHEDULED_AUCTION_TIME,lambda pkt:pkt.messageType==A),
			ConditionalField(AUCTION_BOOK_CLEARING_PRICE,lambda pkt:pkt.messageType==A),
			ConditionalField(COLLAR_REFERENCE_PRICE,lambda pkt:pkt.messageType==A),
			ConditionalField(LOWER_AUCTION_COLLAR,lambda pkt:pkt.messageType==A),
			ConditionalField(UPPER_AUCTION_COLLAR,lambda pkt:pkt.messageType==A)
		]
	
	#data formating
	def toDict(self) :
		d = dict((k, v) for k, v in self.fields.items() if v)
		d["timestamp"] = str(datetime.datetime.utcfromtimestamp(self.timestamp/1000000000.0)),
		if self.symbol :
			d["symbol"] = self.symbol.decode('utf-8').strip()

		
		if self.flagNames :
			messageFlag = {}
			for flagName in FLAGS[self.messageType] :
				messageFlag[flagName] = 1 if FLAGS[self.messageType][flagName]&self.flagNames else 0
			d["flags"] = messageFlag

		if self.reason :
			d["reason"] = self.reason.decode('utf-8').strip()

		d["messageType"] = chr(self.messageType)
		return d

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
				"messages" : [(msg.toDict()) for msg in self.messages]
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
				#print (segment.messages)
				outputfile.write(bytes(json.dumps(segment.toDict())+"\n",'UTF-8'))


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
				with open(smallerJsonFilename,'rb') as smallerJson :
					for line in smallerJson :
						bigJson.write(line)
		print("all done!")
	finally : 
		print ("deleting %s"%workspace)
		shutil.rmtree(workspace)


if __name__ == '__main__':

	main()

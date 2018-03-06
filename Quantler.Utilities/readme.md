**Utilities useable for Quantler**
------------------------------

DataCapture
------------------------------
Used for capturing and storing data from a feed. It stores data as it is arrived by a specified datafeed.

Program options:

f: Name of the datafeed instance to use, example: Quantler.DataFeeds.BittrexPublic.BittrexDataFeed
d: Destination folder for the captured data to be stored

DataConverter
------------------------------
Converts data as retrieved by the datacapture logic into Quantler compatible DAT files.


IEXHistoricalData
------------------------------
Used for retrieving historical data from IEX.
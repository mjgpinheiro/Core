<p align="center">
  <img src="http://localhost:8090/files/images/logo_dark.png" alt="Quantler Logo"/>
</p>

**NOTE: QUANTLER IS CURRENTLY UNDER HEAVY DEVELOPMENT, APIs ARE SUBJECT TO CHANGE WITHOUT PRIOR NOTICE, CODE MIGHT NOT WORK AS EXPECTED AND CAN EVEN BE INCOMPLETE!**

Quantler Core is the main library used for creating, backtesting, analyzing and running trading algorithms also called Quant Funds. It is written in C# and makes use of .net core making it compatible with different environments (Windows, Linux, MacOS). It allows Quant Funds to be easily created on a modular fashion and backtested against historical data (with minute and tick-by-tick resolution), providing analytics and insights regarding a particular Quant Fund's performance. Quantler also supports live-trading of different asset types (crypto, stocks and ETFs) against different exchanges (Binance, Cobinhood, HitBtc). Quantler empowers the individual to create, share and improve their very own Quant Funds that can run in any environment. To learn more about Quantler and get insights on public amd open source developed Quant Funds, please visit [Quantler](https://www.Quantler.com).

## Overview
- Modular: by allowing Quant Funds to be created in a modular fashion, those who cannot program can reuse existing components, allow for a larger audience to create Quant Funds. Expert users can reuse existing components as well to decrease development time or experiment with different models with ease.
- Framework: Quantler aims to be a framework where you can build any Quant Fund quickly and with a layer of abstraction. This allows you to focus on developing Quant Funds and not develop everything around your ideas (connections with brokers, data integration, helping functions etc..)
- Secure: You are the owner of your trading account, the connection with your brokerage account is private
- Backtesting, Sim- and LiveTrading: Run a backtest or run your Quant Fund in a controlled and safe environment with fake money to see how it performs. Attach your Quant Fund to your brokerage account when you feel comfortable.
- Extensible: many of Quantler's features are implemented in a way for easier extensibility. Want to implement your own brokerage connection or a different way of handling orders? You can easily change this by selecting your implementation when running a Quant Fund.

## Github Bounties

Github bounties are currently not implement yet. You are free to help out with the current development of Quantler as there is much to be done before being fully operational.

## Getting Started

Option 1: [Download](https://github.com/Quantler/Core/archive/master.zip) the latest version via github and extract the zip file to your chosen location

Option 2: Install [Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git) and clone the repo:

```
git clone https://github.com/Quantler/Core.git
cd Core
```

### Prerequisites

In order to compile and run Quantler Core you will need to install the latest version of the .NET Core SDK. 

You can download the .NET Core SDK directly from [Microsoft](https://www.microsoft.com/net/learn/get-started/windows)

### Installing

#### Windows
1. Make sure you have downloaded and installed the latest version of the .NET Core SDK (see Prerequisites)
2. Install [Visual Studio](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx)
3. Open ```Quantler.Core.sln``` in Visual Studio
4. Build the solution by clicking Build (top menu) -> Build Solution (or press F6)
5. Press F5 to run your solution

#### macOS
1. Make sure you have downloaded and installed the latest version of the .NET Core SDK (see Prerequisites)
2. Install [Visual Studio](https://www.visualstudio.com/vs/visual-studio-mac/)
3. Open ```Quantler.Core.sln``` in Visual Studio
4. After opening the Quantler Core solution, Visual Studio should automatically restore all dependent Nuget packages. If this is not the case, you can initiate this process manually: In the top menu bar, click Project -> Restore NuGet Packages.
5. To run your solution, press Run -> Start Debugging in the top menu bar

#### Linux (Debian, Ubuntu)

##### Desktop
1. Make sure you have downloaded and installed the latest version of the .NET Core SDK (see Prerequisites)
2. Install [Visual Studio Code](https://www.visualstudio.com/vs/visual-studio-mac/)
3. Open ```Quantler.Core.sln``` in Visual Studio
4. Open a terminal ```CTRL+` ``` and type ```dotnet restore``` to restore all NuGet packages
5. Press F5 to run your solution

##### Headless/Server
1. Make sure you have downloaded and installed the latest version of the .NET Core SDK (see Prerequisites)
2. In the solution folder of Quantler Core, type the following command to restore all packages:
``` dotnet restore ```
3. Compile your current solution:
``` dotnet build .```
4. Navigate to your build version ```cd Quantler.Run/bin/Debug/netcoreapp2.0```
5. Run your version ```dotnet Quantler.Run.dll```


#### Docker
Easiest way to run Quantler Core is to make use of docker, use the following docker command to run a new instance:
```
docker create \
	--name QuantlerCore \
	-v </path/to/config>:/app/Quantler.Run/bin/release/UserConfig \
	-v </path/to/library>:/app/Quantler.Run/bin/release/Custom:ro \
	quantler/core
```

## Running tests

In order to run all unit tests associated with the project, run the following commands in the main solution folder:

```
cd Quantler.Tests
dotnet test .
```

## Deployment

Quantler Core is very agile when it comes to deployment capabilities. 

### Global.json
In the Global.json config file you can specify which implementations to use when running an instance.

There 2 modes you can run an instance in:
1. Backtester (for running simulations)
2. LiveTrading (for running your Quant Funds against a live broker or simulated broker)

### Portfolio.json

In the Portfolio.json file you can specify how your Quant Fund should look like (which modules, broker type, universe, parameters etc..)


## Built With

* [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp/) - Extremely Fast MessagePack Serializer for C#(.NET, .NET Core, Unity, Xamarin). / msgpack.org\[C#\]
* [NLog](http://nlog-project.org/) - Flexible & free open-source logging for .NET
* [NetMQ](https://github.com/zeromq/netmq) - A 100% native C# implementation of ZeroMQ for .NET
* [Jil](https://github.com/kevin-montrose/Jil) - Fast .NET JSON (De)Serializer, Built On Sigil
* [Flurl](http://tmenier.github.io/Flurl/) - Flurl is a modern, fluent, asynchronous, testable, portable, buzzword-laden URL builder and HTTP client library.
* [XUnit](https://github.com/xunit/xunit) - A free, open source, community-focused unit testing tool for the .NET Framework.
* [MoreLINQ](https://morelinq.github.io/) - Extensions to LINQ to Objects
* [Polly](https://github.com/App-vNext/Polly) - A .NET resilience and transient-fault-handling library
* [NodaTime](https://nodatime.org/) - A better date and time API for .NET

## Contributing

Please read [CONTRIBUTING](https://github.com/Quantler/Core/blob/master/Contributing.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/Quantler/Core/tags). 

## License

This project is licensed under the Apache 2.0 License - see the [LICENSE](https://github.com/Quantler/Core/blob/master/LICENSE) file for details

## Acknowledgments

* Large parts of the Indicator, Order Flow and Data logic is based on the QuantConnect LEAN library, see [LEAN](https://github.com/QuantConnect/Lean) which allows you to build and lease your Trading Algorithms to third-parties, LEAN is also compatible with Options and Futures (which Quantler is not!).
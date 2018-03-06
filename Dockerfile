# Current version is based on dotnet core 2.0
FROM microsoft/dotnet:2.0
WORKDIR /app

#Please use GitHub for questions and issues
MAINTAINER Quantler <support@quantler.com>

# Uncomment below to make use of the latest version on GitHub (comment local version)
# RUN \
#	wget https://github.com/Qantler/Core/archive/master.zip && \
#	unzip master.zip .

# Run Local Version
COPY ./Quantler.Run/bin/Release /app/Quantler.Run/bin/Release

# Run App
WORKDIR /app/Quantler.Run/bin/Release
ENTRYPOINT ["dotnet", "Quantler.Run.dll"] # Run app
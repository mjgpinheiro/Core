# current version is based on dotnet core 2.0
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# option 1: Run the latest version from GitHub 
# (you can comment out the function below if you want to make use of option 2)
RUN \
	wget https://github.com/Quantler/Core/archive/master.zip && \
	apt-get update && \
	apt-get install unzip -y && \
	unzip master.zip -d . && \
	mv Core-master/* /app

# option 2: Uncomment below to make use of your local version instead of the GitHub version
# COPY . /app

# restore packages
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN cd Quantler.Run && \
	dotnet publish -c Release -o out -f netcoreapp2.0

# build runtime image
FROM microsoft/dotnet:runtime
WORKDIR /app
COPY --from=build-env /app/Quantler.Run/out ./
ENTRYPOINT ["dotnet", "Quantler.Run.dll"] # Run app
# First stage
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /DockerSource

# Copy csproj and restore as distnict layers
COPY *.sln .
COPY Calculations1/*.csproj ./Calculations1/
RUN dotnet restore

# Copy everything else and build website
COPY Calculations1/. ./Calculations1/
WORKDIR /DockerSource/Calculations1
RUN dotnet publish -c release -o /DockerOutput/Website --no-restore

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /DockerOutput/Website
COPY --from=build /DockerOutput/Website ./
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000
ENTRYPOINT ["dotnet", "Calculations1.dll"]


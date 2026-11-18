# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base

WORKDIR /app

# .NET 8 container images default to 8080; override to listen on 80.
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Example9WeekCityInfoAPI/Example9WeekCityInfoAPI.csproj", "Example9WeekCityInfoAPI/"]

RUN dotnet restore "./Example9WeekCityInfoAPI/Example9WeekCityInfoAPI.csproj"

COPY . .

WORKDIR "/src/Example9WeekCityInfoAPI"

RUN dotnet build "./Example9WeekCityInfoAPI.csproj" \
    -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./Example9WeekCityInfoAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Final stage
FROM base AS final

# Port 80 is a privileged port on Linux.
USER root

WORKDIR /app

COPY --from=publish /app/publish .

ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Example9WeekCityInfoAPI.dll"]

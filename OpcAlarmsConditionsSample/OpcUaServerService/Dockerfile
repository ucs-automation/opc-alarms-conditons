FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OpcUaServerService/OpcUaServerService.csproj", "OpcUaServerService/"]
RUN dotnet restore "OpcUaServerService/OpcUaServerService.csproj"
COPY . .
WORKDIR "/src/OpcUaServerService"
RUN dotnet build "OpcUaServerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpcUaServerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpcUaServerService.dll"]

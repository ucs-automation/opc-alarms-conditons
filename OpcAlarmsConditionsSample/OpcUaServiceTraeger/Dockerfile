FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OpcUaService/OpcUaService.csproj", "OpcUaService/"]
RUN dotnet restore "OpcUaService/OpcUaService.csproj"
COPY . .
WORKDIR "/src/OpcUaService"
RUN dotnet build "OpcUaService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpcUaService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpcUaService.dll"]

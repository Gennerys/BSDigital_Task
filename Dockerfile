# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MetaExchange.Core/MetaExchange.Core.csproj MetaExchange.Core/
COPY MetaExchange/MetaExchange.csproj MetaExchange/
RUN dotnet restore MetaExchange/MetaExchange.csproj

COPY MetaExchange.Core/ MetaExchange.Core/
COPY MetaExchange/ MetaExchange/
RUN dotnet publish MetaExchange/MetaExchange.csproj -c Release -o /app/publish --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080
ENTRYPOINT ["dotnet", "MetaExchange.dll"]

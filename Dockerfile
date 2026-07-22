# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BSDigital_Task.Core/BSDigital_Task.Core.csproj BSDigital_Task.Core/
COPY BSDigital_Task/BSDigital_Task.csproj BSDigital_Task/
RUN dotnet restore BSDigital_Task/BSDigital_Task.csproj

COPY BSDigital_Task.Core/ BSDigital_Task.Core/
COPY BSDigital_Task/ BSDigital_Task/
RUN dotnet publish BSDigital_Task/BSDigital_Task.csproj -c Release -o /app/publish --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080
ENTRYPOINT ["dotnet", "BSDigital_Task.dll"]

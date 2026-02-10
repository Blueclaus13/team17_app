FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0.100-preview.7 AS build
WORKDIR /source

COPY . .
RUN dotnet restore "team17_app.sln"

WORKDIR /source/MindfulMomentsApp
RUN dotnet publish -c Release -o /app/publish --no-restore

# Nginx for Blazor
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0.100-preview.7
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet". "MindfulMomentsApp.dll" ]
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

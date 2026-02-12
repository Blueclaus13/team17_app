FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "team17_app.sln"
WORKDIR /source/MindfulMomentsApp
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MindfulMomentsApp.dll"]
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

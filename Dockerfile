# .NET 10.0 SDK (corrige o erro)
FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0.100-preview.7 AS build
WORKDIR /source

COPY . .
RUN dotnet restore "team17_app.sln"

WORKDIR /source/MindfulMomentsApp
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Nginx para servir Blazor
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy everything
COPY . .

# Restore solution
RUN dotnet restore "team17_app.sln"

# Publish the Blazor app
WORKDIR /source/MindfulMomentsApp
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage (Nginx for static Blazor WASM)
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy published files
COPY --from=build /app/publish/wwwroot .

# Custom nginx config for Blazor routing
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

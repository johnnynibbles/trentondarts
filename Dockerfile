# Stage 1: Build Vite frontend
FROM node:22-alpine AS frontend
WORKDIR /app
COPY TrentonDarts.Web/ClientApp/package*.json ./
RUN npm ci
COPY TrentonDarts.Web/ClientApp ./
# app.css has @source "../Pages/**/*.cshtml" — supply those files so Tailwind generates the full stylesheet
COPY TrentonDarts.Web/Pages ../Pages
RUN npm run build
# outDir '../wwwroot/dist' → output lands at /wwwroot/dist

# Stage 2: Restore .NET dependencies (separate layer for cache efficiency)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY TrentonDarts.Web/TrentonDarts.Web.csproj TrentonDarts.Web/
COPY TrentonDarts.Tests/TrentonDarts.Tests.csproj TrentonDarts.Tests/
RUN dotnet restore TrentonDarts.Web/TrentonDarts.Web.csproj

# Stage 3: Build and publish
FROM restore AS build
COPY . .
COPY --from=frontend /wwwroot/dist ./TrentonDarts.Web/wwwroot/dist
RUN dotnet publish TrentonDarts.Web/TrentonDarts.Web.csproj \
    -c Release -o /app/publish --no-restore /p:SkipClientBuild=true

# Stage 4: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TrentonDarts.Web.dll"]

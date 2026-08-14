# Multi-stage Dockerfile for KasirIn .NET 8 Blazor WebApp & API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY KasirIn.sln ./
COPY src/KasirIn.Domain/KasirIn.Domain.csproj src/KasirIn.Domain/
COPY src/KasirIn.Application/KasirIn.Application.csproj src/KasirIn.Application/
COPY src/KasirIn.Infrastructure/KasirIn.Infrastructure.csproj src/KasirIn.Infrastructure/
COPY src/KasirIn.Api/KasirIn.Api.csproj src/KasirIn.Api/
COPY src/KasirIn.Web/KasirIn.Web.csproj src/KasirIn.Web/
COPY tests/KasirIn.UnitTests/KasirIn.UnitTests.csproj tests/KasirIn.UnitTests/

# Restore dependencies
RUN dotnet restore KasirIn.sln

# Copy all source files
COPY . .

# Run Unit Tests during build
RUN dotnet test KasirIn.sln --no-restore --verbosity normal

# Publish Web App
RUN dotnet publish src/KasirIn.Web/KasirIn.Web.csproj -c Release -o /out/web

# Publish API
RUN dotnet publish src/KasirIn.Api/KasirIn.Api.csproj -c Release -o /out/api

# Production Runtime Stage for Web App
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out/web ./

ENV ASPNETCORE_URLS=http://+:5200
EXPOSE 5200

ENTRYPOINT ["dotnet", "KasirIn.Web.dll"]

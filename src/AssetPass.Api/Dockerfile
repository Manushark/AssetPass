# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for efficient caching
COPY ["src/AssetPass.Domain/AssetPass.Domain.csproj", "src/AssetPass.Domain/"]
COPY ["src/AssetPass.Application/AssetPass.Application.csproj", "src/AssetPass.Application/"]
COPY ["src/AssetPass.Infrastructure/AssetPass.Infrastructure.csproj", "src/AssetPass.Infrastructure/"]
COPY ["src/AssetPass.Api/AssetPass.Api.csproj", "src/AssetPass.Api/"]

RUN dotnet restore "src/AssetPass.Api/AssetPass.Api.csproj"

# Copy all source files and build
COPY src/ src/
WORKDIR "/src/src/AssetPass.Api"
RUN dotnet publish "AssetPass.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Create non-root user and uploads directory
RUN mkdir -p /app/uploads && chown -R $APP_UID:$APP_UID /app
USER $APP_UID

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssetPass.Api.dll"]

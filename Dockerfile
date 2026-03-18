# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj first
COPY InventoryManagementAPI.csproj ./
RUN dotnet restore

# Copy rest of files
COPY . ./

# Publish explicitly
RUN dotnet publish InventoryManagementAPI.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published files
COPY --from=build /app/out ./

# Run app
ENTRYPOINT ["dotnet", "InventoryManagementAPI.dll"]
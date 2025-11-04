# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the whole repo (so ProjectReferences like OrionBus.Contracts are available)
COPY . .
# Restore & publish the InventoryService project
RUN dotnet restore "src/OrionBus.InventoryService/OrionBus.InventoryService.csproj"
RUN dotnet publish "src/OrionBus.InventoryService/OrionBus.InventoryService.csproj" -c Release -o /out /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "OrionBus.InventoryService.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/
COPY ../Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/
COPY ./*.csproj ./
RUN dotnet restore ./

# Copy all
COPY ../BuildingBlocks ./BuildingBlocks
COPY ../Shared.Contracts ./Shared.Contracts
COPY . .

RUN dotnet publish -c Release -o /app

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT [ "dotnet", "Inventory.API.dll" ]

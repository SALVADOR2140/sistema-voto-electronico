# 1. IMAGEN BASE
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000
ENV ASPNETCORE_HTTP_PORTS=10000

# 2. COMPILACIÓN
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SistemaVotoElectronico.Api/SistemaVotoElectronico.Api.csproj", "SistemaVotoElectronico.Api/"]

RUN dotnet restore "SistemaVotoElectronico.Api/SistemaVotoElectronico.Api.csproj"

COPY . .
WORKDIR "/src/SistemaVotoElectronico.Api"
RUN dotnet build "SistemaVotoElectronico.Api.csproj" -c Release -o /app/build

# 3. PUBLICACIÓN
FROM build AS publish
RUN dotnet publish "SistemaVotoElectronico.Api.csproj" -c Release -o /app/publish

# 4. FINAL
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVotoElectronico.Api.dll"]
# 1. Imagen base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Configuración del puerto (¡Esto es lo importante que ya logramos!)
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

# 2. Imagen de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# Ahora nos movemos a la carpeta de la API para compilar
WORKDIR "/src/SistemaVotoElectronico.Api"

# Restauramos y compilamos (al tener todos los archivos, ya no fallará por faltar referencias)
RUN dotnet restore "SistemaVotoElectronico.Api.csproj"
RUN dotnet build "SistemaVotoElectronico.Api.csproj" -c Release -o /app/build

# 3. Publicación
FROM build AS publish
RUN dotnet publish "SistemaVotoElectronico.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVotoElectronico.Api.dll"]
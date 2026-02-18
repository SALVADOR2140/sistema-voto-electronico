# 1. IMAGEN BASE (Entorno para correr la app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# 2. COMPILACIÓN (Entorno para construir el código)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de proyecto del MVC
# ASEGÚRATE que el nombre de la carpeta coincida con el tuyo
COPY ["SistemaVotoElectronico.Web/SistemaVotoElectronico.Web.csproj", "SistemaVotoElectronico.Web/"]

# Si tu proyecto MVC usa una librería compartida (ej. "Shared"), descomenta la línea de abajo:
# COPY ["SistemaVotoElectronico.Shared/SistemaVotoElectronico.Shared.csproj", "SistemaVotoElectronico.Shared/"]

# Restaura las dependencias
RUN dotnet restore "SistemaVotoElectronico.Web/SistemaVotoElectronico.Web.csproj"

# Copia todo el resto del código
COPY . .
WORKDIR "/src/SistemaVotoElectronico.Web"
RUN dotnet build "SistemaVotoElectronico.Web.csproj" -c Release -o /app/build

# 3. PUBLICACIÓN
FROM build AS publish
RUN dotnet publish "SistemaVotoElectronico.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. FINAL
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVotoElectronico.Web.dll"]
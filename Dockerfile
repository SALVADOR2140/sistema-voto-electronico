# 1. Imagen base (la que ejecuta la app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# 2. Imagen de construcción (la que compila el código)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos de proyecto (ajustado a tus carpetas)
COPY ["SistemaVotoElectronico.Api/SistemaVotoElectronico.Api.csproj", "SistemaVotoElectronico.Api/"]
COPY ["SistemaVotoElectronico.Modelos/SistemaVotoElectronico.Modelos.csproj", "SistemaVotoElectronico.Modelos/"]
COPY ["SistemaVotoElectronico.Negocio/SistemaVotoElectronico.Negocio.csproj", "SistemaVotoElectronico.Negocio/"]
COPY ["SistemaVotoElectronico.Datos/SistemaVotoElectronico.Datos.csproj", "SistemaVotoElectronico.Datos/"]

# Restauramos dependencias
RUN dotnet restore "SistemaVotoElectronico.Api/SistemaVotoElectronico.Api.csproj"

# Copiamos todo lo demás
COPY . .

# Compilamos
WORKDIR "/src/SistemaVotoElectronico.Api"
RUN dotnet build "SistemaVotoElectronico.Api.csproj" -c Release -o /app/build

# 3. Publicación
FROM build AS publish
RUN dotnet publish "SistemaVotoElectronico.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Etapa final (la que va a Render)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# --- EL TRUCO PARA RENDER ---

ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
# ----------------------------

ENTRYPOINT ["dotnet", "SistemaVotoElectronico.Api.dll"]
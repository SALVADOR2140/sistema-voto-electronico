# 1. IMAGEN BASE
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Usamos el puerto 10000 para estandarizar con tu API
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

# 2. COMPILACIÓN
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- CORRECCIÓN 1: Nombres de carpetas exactos ---
# Copiamos el proyecto MVC (Cambiamos "Web" por "MVC")
COPY ["SistemaVotoElectronico.MVC/SistemaVotoElectronico.MVC.csproj", "SistemaVotoElectronico.MVC/"]

# --- CORRECCIÓN 2: Agregamos dependencias ---
# Seguramente el MVC usa los Modelos, así que debemos copiarlos también
COPY ["SistemaVotoElectronico.Modelos/SistemaVotoElectronico.Modelos.csproj", "SistemaVotoElectronico.Modelos/"]

# Restauramos las dependencias del MVC
RUN dotnet restore "SistemaVotoElectronico.MVC/SistemaVotoElectronico.MVC.csproj"

# Copiamos todo el código restante
COPY . .

# Nos movemos a la carpeta correcta
WORKDIR "/src/SistemaVotoElectronico.MVC"

# Compilamos
RUN dotnet build "SistemaVotoElectronico.MVC.csproj" -c Release -o /app/build

# 3. PUBLICACIÓN
FROM build AS publish
RUN dotnet publish "SistemaVotoElectronico.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. FINAL
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# --- CORRECCIÓN 3: Nombre del DLL final ---
# El archivo compilado se llamará .MVC.dll, no .Web.dll
ENTRYPOINT ["dotnet", "SistemaVotoElectronico.MVC.dll"]
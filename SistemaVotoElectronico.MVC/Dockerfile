# 1. IMAGEN BASE
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Configuramos el puerto 10000 para Render
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

# 2. COMPILACIÓN
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos TODO el repositorio para que encuentre todos los proyectos (MVC, Modelos, etc.)
COPY . .

# Nos movemos a la carpeta del proyecto MVC
WORKDIR "/src/SistemaVotoElectronico.MVC"

# Restauramos y compilamos
RUN dotnet restore "SistemaVotoElectronico.MVC.csproj"
RUN dotnet build "SistemaVotoElectronico.MVC.csproj" -c Release -o /app/build

# 3. PUBLICACIÓN (AQUÍ ESTÁ LA CORRECCIÓN)
FROM build AS publish
# Agregamos /p:ErrorOnDuplicatePublishOutputFiles=false para que no falle si hay archivos repetidos
RUN dotnet publish "SistemaVotoElectronico.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

# 4. FINAL
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVotoElectronico.MVC.dll"]
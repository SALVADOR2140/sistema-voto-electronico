# 1. IMAGEN BASE
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

# 2. COMPILACIÓN
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- EL CAMBIO SALVADOR ---
# En lugar de copiar csproj por csproj, copiamos TODO de una vez.
# Así Docker encuentra la carpeta "Modelos" sin problemas.
COPY . .
# --------------------------

# Nos movemos a la carpeta del MVC
WORKDIR "/src/SistemaVotoElectronico.MVC"

# Restauramos y compilamos
RUN dotnet restore "SistemaVotoElectronico.MVC.csproj"
RUN dotnet build "SistemaVotoElectronico.MVC.csproj" -c Release -o /app/build

# 3. PUBLICACIÓN
FROM build AS publish
RUN dotnet publish "SistemaVotoElectronico.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. FINAL
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVotoElectronico.MVC.dll"]
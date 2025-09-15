# Étape 1 : Image de base pour exécution
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Étape 2 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier le projet et restaurer
COPY ["TodoList.Server.csproj", "."]
RUN dotnet restore "./TodoList.Server.csproj"

# Copier tout le code et compiler
COPY . .
RUN dotnet build "TodoList.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Étape 3 : Publication
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TodoList.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape 4 : Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoList.Server.dll"]

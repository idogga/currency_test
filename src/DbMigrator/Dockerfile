# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["DbMigrator.csproj", "./"]
RUN dotnet restore "DbMigrator.csproj"

# Копируем остальные файлы и собираем проект
COPY . .
RUN dotnet publish "DbMigrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DbMigrator.dll"]
# 1. Базовый образ для .NET 6 (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80   # Порт, который будет слушать контейнер
EXPOSE 443  # Для HTTPS (если нужно)

# 2. Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# 2.1 Копируем файлы проекта и восстанавливаем зависимости
COPY ["Demirbaslar.API/Demirbaslar.API.csproj", "Demirbaslar.API/"]
RUN dotnet restore "Demirbaslar.API/Demirbaslar.API.csproj"

# 2.2 Копируем весь код и собираем
COPY . .
WORKDIR "/src/Demirbaslar.API"
RUN dotnet build -c Release -o /app/build

# 3. Публикуем приложение
FROM build AS publish
RUN dotnet publish -c Release -o /app/publishj" -c Release -o /app/publish

# 4. Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 5. Точка входа (запуск приложения)
ENTRYPOINT ["dotnet", "Demirbaslar.API.dll"]
# Базовый образ для .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов
COPY ["Demirbaslar.API/Demirbaslar.API.csproj", "Demirbaslar.API/"]
RUN dotnet restore "Demirbaslar.API/Demirbaslar.API.csproj"

# Копируем остальные файлы и собираем
COPY . .
WORKDIR "/src/Demirbaslar.API"
RUN dotnet build -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demirbaslar.API.dll"]
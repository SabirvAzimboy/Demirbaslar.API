# Базовый образ для .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Сборка с .NET 8.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только файлы проектов (для кэширования)
COPY ["Demirbaslar.API/Demirbaslar.API.csproj", "Demirbaslar.API/"]
RUN dotnet restore "Demirbaslar.API/Demirbaslar.API.csproj"

# Копируем остальные файлы
COPY . .
WORKDIR "/src/Demirbaslar.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demirbaslar.API.dll"]
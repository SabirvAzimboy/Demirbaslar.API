# Базовый образ для .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
RUN apt-get update \
	&& apt-get install -y --no-install-recommends \
	clang zlibig-dev
ARG BUILD_CONFIGURATIN=Release
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Сборка с .NET 8.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Установка .NET 8.0 SDK (если Railway игнорирует runtime.txt)
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 8.0.100 --install-dir /usr/share/dotnet
ENV PATH="/usr/share/dotnet:${PATH}"

# Копируем только файлы проектов (для кэширования)
COPY ["Demirbaslar.API/Demirbaslar.API.csproj", "Demirbaslar.API/"]
RUN dotnet restore "Demirbaslar.API/Demirbaslar.API.csproj"

# Копируем остальные файлы
COPY . .
WORKDIR "/src/Demirbaslar.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATIN=Release
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demirbaslar.API.dll"]
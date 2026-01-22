# Estágio 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia ficheiro do projeto primeiro (para otimização de cache)
COPY ["src/RestaurantReservations.API/RestaurantReservations.API.csproj", "src/RestaurantReservations.API/"]
RUN dotnet restore "src/RestaurantReservations.API/RestaurantReservations.API.csproj"

# Copia resto do código
COPY . .
WORKDIR "/src/src/RestaurantReservations.API"
RUN dotnet build "RestaurantReservations.API.csproj" -c Release -o /app/build

# Estágio 2: PUBLISH
FROM build AS publish
RUN dotnet publish "RestaurantReservations.API.csproj" -c Release -o /app/publish

# Estágio 3: RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Configurações
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestaurantReservations.API.dll"]
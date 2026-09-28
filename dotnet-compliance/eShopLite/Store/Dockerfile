FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY "eShopLite/DataEntities/DataEntities.csproj" "./DataEntities/DataEntities.csproj"
COPY "eShopLite/Store/Store.csproj" "./Store/Store.csproj"
RUN dotnet restore "./Store/Store.csproj"

COPY "eShopLite/DataEntities" "./DataEntities"
COPY "eShopLite/Store" "./Store"
RUN dotnet publish "./Store/Store.csproj" -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Store.dll"]
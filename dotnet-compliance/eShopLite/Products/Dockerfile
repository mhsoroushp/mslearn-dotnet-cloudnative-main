FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY "eShopLite/DataEntities/DataEntities.csproj" "./DataEntities/DataEntities.csproj"
COPY "eShopLite/Products/Products.csproj" "./Products/Products.csproj"
RUN dotnet restore "./Products/Products.csproj"

COPY "eShopLite/DataEntities" "./DataEntities"
COPY "eShopLite/Products" "./Products"
RUN dotnet publish "./Products/Products.csproj" -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Products.dll"]
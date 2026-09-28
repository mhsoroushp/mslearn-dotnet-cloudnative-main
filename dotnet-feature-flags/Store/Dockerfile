FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["DataEntities/DataEntities.csproj", "DataEntities/"]
COPY ["Store/Store.csproj", "Store/"]
RUN dotnet restore "Store/Store.csproj"

COPY DataEntities/ DataEntities/
COPY Store/ Store/
WORKDIR /src/Store
RUN dotnet publish "Store.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.dll"]
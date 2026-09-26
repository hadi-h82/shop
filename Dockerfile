FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY shop-server/ ./shop-server/

RUN dotnet restore "shop-server/Sevart.Api/Sevart.Api.csproj"

RUN dotnet publish "shop-server/Sevart.Api/Sevart.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8000

ENTRYPOINT ["dotnet", "Sevart.Api.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/app.api.csproj", "src/app.api/"]
RUN dotnet restore "src/app.api.csproj"
COPY . .
WORKDIR "/src/ordersoutbox/app.api"
RUN dotnet build "./app.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./app.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "app.api.dll"]

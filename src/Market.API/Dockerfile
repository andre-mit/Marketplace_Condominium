FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MarktplaceCondo.sln", "src"]
COPY ["src/Market.Domain/Market.Domain.csproj", "src/Market.Domain/"]
COPY ["src/Market.SharedApplication/Market.SharedApplication.csproj", "src/Market.SharedApplication/"]
COPY ["src/Market.API/Market.API.csproj", "src/Market.API/"]
RUN dotnet restore "MarktplaceCondo.sln"
    
COPY . .
WORKDIR "/src/Market.API"
RUN dotnet build "Market.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Market.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Market.API.dll"]

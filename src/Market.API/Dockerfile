FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copy solution and project files (paths are relative to repo root because build context is set to project root in docker-compose)
COPY ["MarktplaceCondo.sln", "."]
COPY ["src/Market.Domain/Market.Domain.csproj", "src/Market.Domain/"]
COPY ["src/Market.Application/Market.SharedApplication.csproj", "src/Market.Application/"]
COPY ["src/Market.API/Market.API.csproj", "src/Market.API/"]

RUN dotnet restore "MarktplaceCondo.sln"

# copy all sources
COPY . .

# set working directory to the API project folder (after COPY . ., sources are at /src/src/...)
WORKDIR "/src/src/Market.API"
RUN dotnet build "Market.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/Market.API"
RUN dotnet publish "Market.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Market.API.dll"]

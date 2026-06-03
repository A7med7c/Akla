# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Restaurants.Api/Restaurants.Api.csproj", "Restaurants.Api/"]
COPY ["Restaurants.Application/Restaurants.Application.csproj", "Restaurants.Application/"]
COPY ["Restaurants.Dpomain/Restaurants.Domain.csproj", "Restaurants.Dpomain/"]
COPY ["Restaurants.Infrastructure/Restaurants.Infrastructure.csproj", "Restaurants.Infrastructure/"]
RUN dotnet restore "./Restaurants.Api/Restaurants.Api.csproj"
COPY . .
WORKDIR "/src/Restaurants.Api"
RUN dotnet build "./Restaurants.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Restaurants.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Restaurants.Api.dll"]
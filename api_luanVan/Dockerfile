# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#section 1
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["api_LuanVan/api_LuanVan.csproj", "api_LuanVan/"]
RUN dotnet restore "./api_LuanVan/api_LuanVan.csproj"
COPY . .
WORKDIR "/src/api_LuanVan"
RUN dotnet build "./api_LuanVan.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./api_LuanVan.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api_LuanVan.dll"]

#section 2

#this section used to self deploy without visual studio
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#
#COPY ["api_LuanVan.csproj", "."]
#RUN dotnet restore "api_LuanVan.csproj"
#COPY . .
#
## Build project
#RUN dotnet build "./api_LuanVan.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## Publish project
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "api_LuanVan.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "api_LuanVan.dll"]
#
#docker build -t apiluanvan:manual .
#docker run -dit -p 32780:80 --name apiluanvan-container apiluanvan:manual

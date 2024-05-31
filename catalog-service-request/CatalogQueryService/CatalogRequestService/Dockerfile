#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App
COPY ../ ./
#COPY ../../../../../rabbit-utilities/RabbitUtilities/bin/Debug/net6.0/RabbitUtilities.dll ./

RUN dotnet restore 
RUN dotnet publish -c Release -o out

FROM base AS final
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "CatalogRequestService.dll"]

#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
##COPY ["CatalogRequestService/CatalogRequestService.csproj", "CatalogRequestService/"]
#COPY ../ ./
#RUN dotnet restore "./CatalogRequestService/CatalogRequestService.csproj"
#COPY . .
#WORKDIR "/src/CatalogRequestService"
#RUN dotnet build "./CatalogRequestService.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./CatalogRequestService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "CatalogRequestService.dll"]
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
ENTRYPOINT ["dotnet", "CatalogQueryService.dll"]
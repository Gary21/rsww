﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY rabbit-utilities/RabbitUtilities/bin/Debug/net6.0/RabbitUtilities.dll RabbitUtilities.dll
COPY api-gateway ./
#COPY ["api-gateway/api-gateway.csproj", "./"]
RUN dotnet restore "api-gateway.csproj"
WORKDIR "/src/"
#RUN dotnet build "api-gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api-gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api-gateway.dll"]

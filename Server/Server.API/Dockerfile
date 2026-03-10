# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

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
COPY ["Server/Server.API/Server.API.csproj", "Server/Server.API/"]
COPY ["Server/Server.Domain/Server.Domain.csproj", "Server/Server.Domain/"]
COPY ["Server/Server.Service/Server.Service.csproj", "Server/Server.Service/"]
COPY ["Common/Common.Service/Common.Service.csproj", "Common/Common.Service/"]
COPY ["Common/Common.Domain/Common.Domain.csproj", "Common/Common.Domain/"]
COPY ["Common/Common.Repository/Common.Repository.csproj", "Common/Common.Repository/"]
RUN dotnet restore "./Server/Server.API/Server.API.csproj"
COPY . .
WORKDIR "/src/Server/Server.API"
RUN dotnet build "./Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.API.dll"]
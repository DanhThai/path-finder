FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

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

# Build the EF migration bundle
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle -p /src/Common/Common.Repository/Common.Repository.csproj -s /src/Server/Server.API/Server.API.csproj -o /app/build/efbundle --self-contained

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build --chmod=+x /app/build/efbundle .
COPY --from=build --chmod=+x /src/Server/Server.API/entrypoint.sh .
ENTRYPOINT ["./entrypoint.sh"]
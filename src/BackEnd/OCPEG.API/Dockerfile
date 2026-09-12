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
COPY ["src/BackEnd/OCPEG.API/OCPEG.API.csproj", "src/BackEnd/OCPEG.API/"]
COPY ["src/BackEnd/OCEPG.Infrastructure/OCEPG.Infrastructure.csproj", "src/BackEnd/OCEPG.Infrastructure/"]
COPY ["src/BackEnd/OCPEG.Application/OCPEG.Application.csproj", "src/BackEnd/OCPEG.Application/"]
COPY ["src/BackEnd/OCPEG.Domain.Enum/OCPEG.Domain.Enum.csproj", "src/BackEnd/OCPEG.Domain.Enum/"]
COPY ["src/BackEnd/OCPEG.Domain/OCPEG.Domain.csproj", "src/BackEnd/OCPEG.Domain/"]
COPY ["src/BackEnd/OCPEG.Framework/OCPEG.Framework.csproj", "src/BackEnd/OCPEG.Framework/"]
RUN dotnet restore "./src/BackEnd/OCPEG.API/OCPEG.API.csproj"
COPY . .
WORKDIR "/src/src/BackEnd/OCPEG.API"
RUN dotnet build "./OCPEG.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OCPEG.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OCPEG.API.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["Src/BankMore.Services.Api.ContasCorrentes/BankMore.Services.Api.ContasCorrentes.csproj", "Src/BankMore.Services.Api.ContasCorrentes/"]
COPY ["Src/BankMore.Application.ContasCorrentes/BankMore.Application.ContasCorrentes.csproj", "Src/BankMore.Application.ContasCorrentes/"]
COPY ["Src/BankMore.Domain.ContasCorrentes/BankMore.Domain.ContasCorrentes.csproj", "Src/BankMore.Domain.ContasCorrentes/"]
COPY ["Src/BankMore.Domain.Common/BankMore.Domain.Common.csproj", "Src/BankMore.Domain.Common/"]
COPY ["Src/BankMore.Domain.Core/BankMore.Domain.Core.csproj", "Src/BankMore.Domain.Core/"]
COPY ["Src/BankMore.Infra.CrossCutting.Bus/BankMore.Infra.CrossCutting.Bus.csproj", "Src/BankMore.Infra.CrossCutting.Bus/"]
COPY ["Src/BankMore.Infra.Data.ContasCorrentes/BankMore.Infra.Data.ContasCorrentes.csproj", "Src/BankMore.Infra.Data.ContasCorrentes/"]
COPY ["Src/BankMore.Infra.Data.Common/BankMore.Infra.Data.Common.csproj", "Src/BankMore.Infra.Data.Common/"]
COPY ["Src/BankMore.Infra.Apis/BankMore.Infra.Apis.csproj", "Src/BankMore.Infra.Apis/"]
COPY ["Src/BankMore.Infra.CrossCutting.Identity/BankMore.Infra.CrossCutting.Identity.csproj", "Src/BankMore.Infra.CrossCutting.Identity/"]
COPY ["Src/BankMore.Infra.CrossCutting.IoC/BankMore.Infra.CrossCutting.IoC.csproj", "Src/BankMore.Infra.CrossCutting.IoC/"]
RUN dotnet restore "./Src/BankMore.Services.Api.ContasCorrentes/BankMore.Services.Api.ContasCorrentes.csproj"
COPY . .
WORKDIR "/src/Src/BankMore.Services.Api.ContasCorrentes"
RUN dotnet build "./BankMore.Services.Api.ContasCorrentes.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankMore.Services.Api.ContasCorrentes.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Services.Api.ContasCorrentes.dll"]
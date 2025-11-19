# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ENV LANG=pt_BR.UTF-8
ENV LC_ALL=pt_BR.UTF-8
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER $APP_UID
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["Src/BankMore.Services.Api.Identidade/BankMore.Services.Api.Identidade.csproj", "Src/BankMore.Services.Api.Identidade/"]
COPY ["Src/BankMore.Application.ContasCorrentes/BankMore.Application.ContasCorrentes.csproj", "Src/BankMore.Application.ContasCorrentes/"]
COPY ["Src/BankMore.Domain.ContasCorrentes/BankMore.Domain.ContasCorrentes.csproj", "Src/BankMore.Domain.ContasCorrentes/"]
COPY ["Src/BankMore.Domain.Common/BankMore.Domain.Common.csproj", "Src/BankMore.Domain.Common/"]
COPY ["Src/BankMore.Domain.Core/BankMore.Domain.Core.csproj", "Src/BankMore.Domain.Core/"]
COPY ["Src/BankMore.Infra.Data.Common/BankMore.Infra.Data.Common.csproj", "Src/BankMore.Infra.Data.Common/"]
COPY ["Src/BankMore.Infra.Data.ContasCorrentes/BankMore.Infra.Data.ContasCorrentes.csproj", "Src/BankMore.Infra.Data.ContasCorrentes/"]
COPY ["Src/BankMore.Infra.CrossCutting.Bus/BankMore.Infra.CrossCutting.Bus.csproj", "Src/BankMore.Infra.CrossCutting.Bus/"]
COPY ["Src/BankMore.Infra.CrossCutting.Identity/BankMore.Infra.CrossCutting.Identity.csproj", "Src/BankMore.Infra.CrossCutting.Identity/"]
COPY ["Src/BankMore.Infra.CrossCutting.IoC/BankMore.Infra.CrossCutting.IoC.csproj", "Src/BankMore.Infra.CrossCutting.IoC/"]
COPY ["Src/BankMore.Infra.Apis/BankMore.Infra.Apis.csproj", "Src/BankMore.Infra.Apis/"]
COPY ["Src/BankMore.Infra.Kafka/BankMore.Infra.Kafka.csproj", "Src/BankMore.Infra.Kafka/"]
RUN dotnet restore "./Src/BankMore.Services.Api.Identidade/BankMore.Services.Api.Identidade.csproj"
COPY . .
WORKDIR "/src/Src/BankMore.Services.Api.Identidade"
RUN dotnet build "./BankMore.Services.Api.Identidade.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankMore.Services.Api.Identidade.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Services.Api.Identidade.dll"]
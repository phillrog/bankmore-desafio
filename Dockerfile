# .NET Core SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Sets the working directory
WORKDIR /app

# Copy Projects
#COPY *.sln .
COPY Src/BankMore.Application.ContasCorrentes/BankMore.Application.ContasCorrentes.csproj ./Src/BankMore.Application.ContasCorrentes/
COPY Src/BankMore.Domain.ContasCorrentes/BankMore.Domain.ContasCorrentes.csproj ./Src/BankMore.Domain.ContasCorrentes/
COPY Src/BankMore.Domain.Core/BankMore.Domain.Core.csproj ./Src/BankMore.Domain.Core/
COPY Src/BankMore.Infra.CrossCutting.Bus/BankMore.Infra.CrossCutting.Bus.csproj ./Src/BankMore.Infra.CrossCutting.Bus/
COPY Src/BankMore.Infra.CrossCutting.Identity/BankMore.Infra.CrossCutting.Identity.csproj ./Src/BankMore.Infra.CrossCutting.Identity/
COPY Src/BankMore.Infra.CrossCutting.IoC/BankMore.Infra.CrossCutting.IoC.csproj ./Src/BankMore.Infra.CrossCutting.IoC/
COPY Src/BankMore.Infra.Data/BankMore.Infra.Data.csproj ./Src/BankMore.Infra.Data/
COPY Src/BankMore.Services.Api.Identidade/BankMore.Services.Api.Identidade.csproj ./Src/BankMore.Services.Api.Identidade/
COPY Src/BankMore.Infra.Apis/BankMore.Infra.Apis.csproj ./Src/BankMore.Infra.Apis/
COPY Directory.Build.props ./Src
COPY Directory.Packages.props ./Src

# .NET Core Restore
RUN dotnet restore ./Src/BankMore.Services.Api.Identidade/BankMore.Services.Api.Identidade.csproj

# Copy All Files
COPY Src ./Src

# .NET Core Build and Publish
RUN dotnet publish ./Src/BankMore.Services.Api.Identidade/BankMore.Services.Api.Identidade.csproj -c Release -o /publish

# ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish ./

# Expose ports
EXPOSE 80
EXPOSE 443

# Setup your variables before running.
ARG MyEnv
ENV ASPNETCORE_ENVIRONMENT $MyEnv

ENTRYPOINT ["dotnet", "BankMore.Services.Api.Identidade.dll"]

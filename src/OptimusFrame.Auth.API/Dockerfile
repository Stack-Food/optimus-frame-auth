FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto primeiro (melhor cache)
COPY src/OptimusFrame.Auth.API/OptimusFrame.Auth.API.csproj src/OptimusFrame.Auth.API/
COPY src/OptimusFrame.Auth.Application/OptimusFrame.Auth.Application.csproj src/OptimusFrame.Auth.Application/
COPY src/OptimusFrame.Auth.Domain/OptimusFrame.Auth.Domain.csproj src/OptimusFrame.Auth.Domain/
COPY src/OptimusFrame.Auth.Infrastructure/OptimusFrame.Auth.Infrastructure.csproj src/OptimusFrame.Auth.Infrastructure/

RUN dotnet restore src/OptimusFrame.Auth.API/OptimusFrame.Auth.API.csproj

# Copiar todo o código
COPY . .

WORKDIR /src/src/OptimusFrame.Auth.API
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OptimusFrame.Auth.API.dll"]

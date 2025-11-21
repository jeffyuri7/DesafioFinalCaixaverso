FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY src/ .

WORKDIR /app/Backend/DesafioFinalCaixaverso.API

RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

RUN groupadd --system appgroup \
	&& useradd --system --gid appgroup --create-home --shell /usr/sbin/nologin appuser

COPY --from=build-env --chown=appuser:appgroup /app/out .

USER appuser

ENTRYPOINT ["dotnet", "DesafioFinalCaixaverso.API.dll"]

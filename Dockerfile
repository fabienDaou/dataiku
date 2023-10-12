FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

# Install node
RUN apt-get update -yq \
    && curl -sL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install curl gnupg nodejs -yq --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

COPY MilleniumFalconChallenge/. .

RUN dotnet build MilleniumFalconChallenge.sln -c Release

FROM build AS publish
RUN dotnet publish MilleniumFalconChallenge.Api/MilleniumFalconChallenge.Api.csproj -c Release --no-build -o /app/webapp

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS release
WORKDIR /app
COPY --chown=millenium:millenium --chmod=u=rX,g=rX --from=publish /app/webapp .
USER 1000
ENTRYPOINT ["dotnet", "MilleniumFalconChallenge.Api.dll"]

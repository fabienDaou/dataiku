FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

# Install node
RUN set -uex; \
    apt-get update; \
    apt-get install -y ca-certificates curl gnupg; \
    mkdir -p /etc/apt/keyrings; \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key \
     | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg; \
    NODE_MAJOR=18; \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_16.x nodistro main" \
     > /etc/apt/sources.list.d/nodesource.list; \
    apt-get update; \
    apt-get install nodejs -y;

COPY MilleniumFalconChallenge/. .

RUN dotnet build MilleniumFalconChallenge.sln -c Release

FROM build AS publish
RUN dotnet publish MFC.Api/MFC.Api.csproj -c Release --no-build -o /app/webapp

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS release
WORKDIR /app
COPY --chown=millenium:millenium --chmod=u=rX,g=rX --from=publish /app/webapp .
USER 1000
ENTRYPOINT ["dotnet", "MFC.Api.dll"]

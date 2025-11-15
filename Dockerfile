FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Inventario.Api/Inventario.Api.csproj Inventario.Api/
COPY Inventario.Core/Inventario.Core.csproj Inventario.Core/
COPY Inventario.Test/Inventario.Test.csproj Inventario.Test/

RUN dotnet restore ./Inventario.Api/Inventario.Api.csproj

COPY Inventario.Api/ ./Inventario.Api/
COPY Inventario.Core/ ./Inventario.Core/
COPY Inventario.Test/ ./Inventario.Test/

COPY .env* /src/

WORKDIR /src/Inventario.Api
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app .

COPY .env* /app/

EXPOSE 5000
ENV ASPNETCORE_URLS="http://+:5000"

ENTRYPOINT ["dotnet", "Inventario.Api.dll"]

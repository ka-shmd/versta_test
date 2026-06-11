FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY VerstaDelivery.Api/VerstaDelivery.Api.csproj VerstaDelivery.Api/
RUN dotnet restore VerstaDelivery.Api/VerstaDelivery.Api.csproj

COPY . .
RUN dotnet publish VerstaDelivery.Api/VerstaDelivery.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VerstaDelivery.Api.dll"]

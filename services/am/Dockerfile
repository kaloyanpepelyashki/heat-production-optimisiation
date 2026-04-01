FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Am.Api/Am.Api.csproj Am.Api/
RUN dotnet restore Am.Api/Am.Api.csproj

COPY Am.Api/ Am.Api/
RUN dotnet publish Am.Api/Am.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Staging
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Am.Api.dll"]

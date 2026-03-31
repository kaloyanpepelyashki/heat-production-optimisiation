FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY services/am/Am.Api/Am.Api.csproj services/am/Am.Api/
RUN dotnet restore services/am/Am.Api/Am.Api.csproj

COPY services/am/Am.Api/ services/am/Am.Api/
RUN dotnet publish services/am/Am.Api/Am.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Am.Api.dll"]

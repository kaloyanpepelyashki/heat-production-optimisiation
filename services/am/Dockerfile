FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY shared/Shared/ shared/Shared/
COPY services/am/Am.Api/ services/am/Am.Api/
RUN dotnet publish services/am/Am.Api/Am.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Am.Api.dll"]

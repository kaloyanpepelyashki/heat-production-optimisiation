FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY shared/Shared/ shared/Shared/
COPY services/sdm/Sdm.Api/ services/sdm/Sdm.Api/
RUN dotnet publish services/sdm/Sdm.Api/Sdm.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sdm.Api.dll"]

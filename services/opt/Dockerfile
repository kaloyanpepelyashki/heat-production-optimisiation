FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY shared/Shared/ shared/Shared/
COPY services/opt/Opt.Api/ services/opt/Opt.Api/
RUN dotnet publish services/opt/Opt.Api/Opt.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Opt.Api.dll"]

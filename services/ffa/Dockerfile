FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY shared/Shared/ shared/Shared/
COPY services/ffa/Ffa.Api/ services/ffa/Ffa.Api/
RUN dotnet publish services/ffa/Ffa.Api/Ffa.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ffa.Api.dll"]

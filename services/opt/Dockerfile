FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY stylecop.json .
COPY Opt.Api/Opt.Api.csproj Opt.Api/
RUN dotnet restore Opt.Api/Opt.Api.csproj

COPY Opt.Api/ Opt.Api/
RUN dotnet publish Opt.Api/Opt.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Opt.Api.dll"]

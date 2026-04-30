FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY services/sdm/stylecop.json .
COPY Sdm.Api/Sdm.Api.csproj Sdm.Api/
RUN dotnet restore Sdm.Api/Sdm.Api.csproj

COPY Sdm.Api/ Sdm.Api/
RUN dotnet publish Sdm.Api/Sdm.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sdm.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Elo.sln ./
COPY Elo.Api/Elo.Api.csproj Elo.Api/
COPY Elo.Application/Elo.Application.csproj Elo.Application/
COPY Elo.Domain/Elo.Domain.csproj Elo.Domain/
COPY Elo.Infrastructure/Elo.Infrastructure.csproj Elo.Infrastructure/

RUN dotnet restore

COPY . .
RUN dotnet publish Elo.Api/Elo.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Elo.Api.dll"]

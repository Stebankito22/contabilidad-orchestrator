FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContabilidadOrchestrator.csproj", "."]
RUN dotnet restore "ContabilidadOrchestrator.csproj"
COPY . .
RUN dotnet build "ContabilidadOrchestrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContabilidadOrchestrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContabilidadOrchestrator.dll"]

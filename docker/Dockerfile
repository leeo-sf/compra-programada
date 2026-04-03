FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
EXPOSE 5000

COPY ["src/CompraProgramada.Api/CompraProgramada.Api.csproj", "CompraProgramada.Api/"]
COPY ["src/CompraProgramada.Application/CompraProgramada.Application.csproj", "CompraProgramada.Application/"]
COPY ["src/CompraProgramada.Data/CompraProgramada.Data.csproj", "CompraProgramada.Data/"]
COPY ["src/CompraProgramada.Domain/CompraProgramada.Domain.csproj", "CompraProgramada.Domain/"]
COPY ["src/CompraProgramada.Infra/CompraProgramada.Infra.csproj", "CompraProgramada.Infra/"]
COPY ["src/CompraProgramada.Shared/CompraProgramada.Shared.csproj", "CompraProgramada.Shared/"]
COPY ["src/CompraProgramada.Worker/CompraProgramada.Worker.csproj", "CompraProgramada.Worker/"]

RUN dotnet restore "CompraProgramada.Api/CompraProgramada.Api.csproj"

COPY . .
RUN dotnet publish "src/CompraProgramada.Api/CompraProgramada.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "CompraProgramada.Api.dll"]
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DatabaseObserver.Worker/DatabaseObserver.Worker.csproj", "DatabaseObserver.Worker/"]
RUN dotnet restore "DatabaseObserver.Worker/DatabaseObserver.Worker.csproj"
COPY . .
WORKDIR "/src/DatabaseObserver.Worker"
RUN dotnet build "DatabaseObserver.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseObserver.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseObserver.Worker.dll"]

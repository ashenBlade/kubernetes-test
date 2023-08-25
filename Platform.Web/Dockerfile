FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Platform.Web/Platform.Web.csproj", "Platform.Web/"]
COPY ["Common.KafkaEvents/Common.KafkaEvents.csproj", "Common.KafkaEvents/"]
RUN dotnet restore "Platform.Web/Platform.Web.csproj"
COPY . .
WORKDIR "/src/Platform.Web"
RUN dotnet build "Platform.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Platform.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Platform.Web.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Commands.Web/Commands.Web.csproj", "Commands.Web/"]
COPY ["Common.KafkaEvents/Common.KafkaEvents.csproj", "Common.KafkaEvents/"]
RUN dotnet restore "Commands.Web/Commands.Web.csproj"
COPY . .
WORKDIR "/src/Commands.Web"
RUN dotnet build "Commands.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Commands.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Commands.Web.dll"]

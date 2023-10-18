FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["KrMicro.Transaction/KrMicro.Transaction.csproj", "KrMicro.Transaction/"]
RUN dotnet restore "KrMicro.Transaction/KrMicro.Transaction.csproj"
COPY . .
WORKDIR "/src/KrMicro.Transaction"
RUN dotnet build "KrMicro.Transaction.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KrMicro.Transaction.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KrMicro.Transaction.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS debug
WORKDIR /source
COPY ["Pokedex/Pokedex.csproj", "Pokedex/"]
RUN dotnet restore "Pokedex/Pokedex.csproj"
COPY . .
WORKDIR "/source/Pokedex"
RUN dotnet build "Pokedex.csproj" -c Release -o /app/build

FROM debug AS publish
RUN dotnet publish "Pokedex.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 80
ENTRYPOINT ["dotnet", "Pokedex.dll"]
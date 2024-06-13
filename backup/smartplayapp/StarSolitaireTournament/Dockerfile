#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
COPY StarSolitaire3.pfx /root/.aspnet/https/
COPY de052f69-79e4-4f62-ad50-82f1abd04add /root/.microsoft/usersecrets/
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StarSolitaireTournament/StarSolitaireTournament.csproj", "StarSolitaireTournament/"]
RUN dotnet restore "StarSolitaireTournament/StarSolitaireTournament.csproj"
COPY . .
WORKDIR "/src/StarSolitaireTournament"
RUN dotnet build "StarSolitaireTournament.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StarSolitaireTournament.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarSolitaireTournament.dll"]
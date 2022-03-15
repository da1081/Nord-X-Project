#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Nord-X-WebApp/Nord-X-WebApp.csproj", "Nord-X-WebApp/"]
COPY ["CVRCore/CVRCore.csproj", "CVRCore/"]
RUN dotnet restore "Nord-X-WebApp/Nord-X-WebApp.csproj"
COPY . .
WORKDIR "/src/Nord-X-WebApp"
RUN dotnet build "Nord-X-WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nord-X-WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nord-X-WebApp.dll"]
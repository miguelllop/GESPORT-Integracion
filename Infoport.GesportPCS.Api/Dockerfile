FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build
WORKDIR /src
COPY ["Infoport.GesportPCS.Api/Infoport.GesportPCS.Api.csproj", "Infoport.GesportPCS.Api/"]
#RUN dotnet restore "Infoport.GesportPCS.Api/Infoport.GesportPCS.Api.csproj"
COPY . .
WORKDIR "/src/Infoport.GesportPCS.Api"
RUN dotnet build "Infoport.GesportPCS.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Infoport.GesportPCS.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Infoport.GesportPCS.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AgriTrace.API/AgriTrace.API.csproj", "AgriTrace.API/"]
RUN dotnet restore "AgriTrace.API/AgriTrace.API.csproj"
COPY . .
RUN dotnet publish "AgriTrace.API/AgriTrace.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AgriTrace.API.dll"]

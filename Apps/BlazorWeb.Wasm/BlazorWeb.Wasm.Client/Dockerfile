FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorWeb.Wasm/BlazorWeb.Wasm.Client/BlazorWeb.Wasm.Client.csproj", "BlazorWeb.Wasm/BlazorWeb.Wasm.Client/"]
RUN dotnet restore "BlazorWeb.Wasm/BlazorWeb.Wasm.Client/BlazorWeb.Wasm.Client.csproj"
COPY . .
WORKDIR "/src/BlazorWeb.Wasm/BlazorWeb.Wasm.Client"
RUN dotnet build "./BlazorWeb.Wasm.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorWeb.Wasm.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorWeb.Wasm.Client.dll"]

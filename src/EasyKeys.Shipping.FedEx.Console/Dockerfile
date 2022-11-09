#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["easykeys.shipping.fedex.console/easykeys.shipping.fedex.console.csproj", "easykeys.shipping.fedex.console/"]
RUN dotnet restore "easykeys.shipping.fedex.console/easykeys.shipping.fedex.console.csproj"
COPY . .
WORKDIR "/src/easykeys.shipping.fedex.console"
RUN dotnet build "easykeys.shipping.fedex.console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "easykeys.shipping.fedex.console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "easykeys.shipping.fedex.console.dll"]

#------------------------------------------------------------------------------------------------------------------------
#
# USAGE:        This Dockerfile builds the sample for hosting application inside of Kubernetes Cluster.
#
# NOTE:         Please run commands from root of the solution.
#
# BUILD:        docker build --rm -f "EasyKeys.Shipping.Stamps.Console/Dockerfile" -t app:EasyKeys.Shipping.Stamps.Console .
#
#
# NOTE:        See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#------------------------------------------------------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EasyKeys.Shipping.Stamps.Console/EasyKeys.Shipping.Stamps.Console.csproj", "EasyKeys.Shipping.Stamps.Console/"]
RUN dotnet restore "EasyKeys.Shipping.Stamps.Console/EasyKeys.Shipping.Stamps.Console.csproj"
COPY . .
WORKDIR "/src/EasyKeys.Shipping.Stamps.Console"
RUN dotnet build "EasyKeys.Shipping.Stamps.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyKeys.Shipping.Stamps.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyKeys.Shipping.Stamps.Console.dll"]

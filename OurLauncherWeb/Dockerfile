FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /OurLauncherWeb

# Copy everything
COPY OurLauncherWeb/*.csproj .
# Restore as distinct layers
RUN dotnet restore
COPY OurLauncherWeb .
# Build and publish a release
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS runtime
COPY --from=build-env /publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "OurLauncherWeb.dll"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /ExampleJTToken7

# Copy everything
COPY ExampleJTToken7/*.csproj .
# Restore as distinct layers
RUN dotnet restore
COPY ExampleJTToken7 .
# Build and publish a release
RUN dotnet publish -c Release -o /publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 443
EXPOSE 80
ENTRYPOINT ["dotnet", "ExampleJTToken7.dll"]
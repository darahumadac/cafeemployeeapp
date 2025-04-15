FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app
# Stage 1 - Publish the api
# Restore dependencies
COPY CafeEmployeeApi.csproj .
RUN dotnet restore
# Copy all other files and publish the api
COPY . .
RUN dotnet publish -c Release -o ./publish

# Stage 2 - Host the api
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT [ "dotnet", "CafeEmployeeApi.dll" ]
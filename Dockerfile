# Use .NET 8 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY GradLink/GradLink.csproj GradLink/
COPY GradLink.Model/GradLink.Model.csproj GradLink.Model/
COPY GradLink.Repository/GradLink.Repository.csproj GradLink.Repository/

RUN dotnet restore GradLink/GradLink.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish GradLink/GradLink.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "GradLink.dll"]

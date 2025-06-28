# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY *.sln .
COPY Authentication.API/Authentication.API.csproj Authentication.API/
COPY Authentication.DAL/Authentication.DAL.csproj Authentication.DAL/
COPY Authentication.Services/Authentication.Services.csproj Authentication.Services/
COPY Authentication.Contracts/Authentication.Contracts.csproj Authentication.Contracts/
COPY Authentication.Utility/Authentication.Utility.csproj Authentication.Utility/

RUN dotnet restore

# Copy all source files
COPY . .

# Build the project
RUN dotnet publish Authentication.API/Authentication.API.csproj -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port you use in Program.cs (usually 80 or 5000)
EXPOSE 80

ENTRYPOINT ["dotnet", "Authentication.API.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PhoneBookApp/PhoneBookApp.csproj", "PhoneBookApp/"]
RUN dotnet restore "PhoneBookApp/PhoneBookApp.csproj"

COPY . .
RUN dotnet publish "PhoneBookApp/PhoneBookApp.csproj" -c Release -o /app/publish

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "PhoneBookApp.dll"]
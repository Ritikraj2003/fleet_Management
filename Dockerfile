# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Saas_Car_Management/Saas_Car_Management.csproj", "Saas_Car_Management/"]
RUN dotnet restore "Saas_Car_Management/Saas_Car_Management.csproj"
COPY . .
WORKDIR "/src/Saas_Car_Management"
RUN dotnet build "Saas_Car_Management.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "Saas_Car_Management.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Saas_Car_Management.dll"]

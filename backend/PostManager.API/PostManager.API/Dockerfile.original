# ===============================
# STEP 1: Build stage
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source và build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ===============================
# STEP 2: Runtime stage
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Copy output từ build stage
COPY --from=build /app/publish .

# Khai báo entrypoint
ENTRYPOINT ["dotnet", "PostManager.API.dll"]
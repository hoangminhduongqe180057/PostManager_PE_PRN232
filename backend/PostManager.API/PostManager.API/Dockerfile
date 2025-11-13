# ===============================
# STEP 1: Build stage
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file và restore
COPY PostManager.API.csproj ./
RUN dotnet restore

# Copy toàn bộ source code
COPY . .

# Build và publish
RUN dotnet publish -c Release -o /app/publish

# ===============================
# STEP 2: Runtime stage
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Copy output từ build stage
COPY --from=build /app/publish .

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "PostManager.API.dll"]
# ===============================
# STEP 1: Build stage
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj trước để cache restore
COPY backend/PostManager.API/PostManager.API/PostManager.API.csproj ./PostManager.API.csproj
RUN dotnet restore "PostManager.API.csproj"

# Copy tất cả source code (trừ file Dockerfile)
COPY backend/PostManager.API/PostManager.API/ ./

# Xóa Dockerfile bị copy nhầm (ngăn dotnet publish đọc phải)
RUN rm -f Dockerfile

# Build và publish ra thư mục /app/publish
RUN dotnet publish -c Release -o /app/publish

# ===============================
# STEP 2: Runtime stage
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PostManager.API.dll"]
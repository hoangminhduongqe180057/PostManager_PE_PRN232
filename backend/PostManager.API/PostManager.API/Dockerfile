# ===============================
# STEP 1: Build stage
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj vào container
COPY backend/PostManager.API/PostManager.API/PostManager.API.csproj ./
RUN dotnet restore "PostManager.API.csproj"

# Copy toàn bộ source code vào container
COPY backend/PostManager.API/PostManager.API/ ./

# Build và publish
RUN dotnet publish -c Release -o /app/publish

# ===============================
# STEP 2: Runtime stage
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PostManager.API.dll"]
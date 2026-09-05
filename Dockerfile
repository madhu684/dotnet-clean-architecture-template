# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/CleanTask.API/CleanTask.API.csproj", "src/CleanTask.API/"]
COPY ["src/CleanTask.Application/CleanTask.Application.csproj", "src/CleanTask.Application/"]
COPY ["src/CleanTask.Infrastructure/CleanTask.Infrastructure.csproj", "src/CleanTask.Infrastructure/"]
COPY ["src/CleanTask.Domain/CleanTask.Domain.csproj", "src/CleanTask.Domain/"]

RUN dotnet restore "src/CleanTask.API/CleanTask.API.csproj"

# Copy source and build
COPY . .
WORKDIR "/src/src/CleanTask.API"
RUN dotnet build "CleanTask.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CleanTask.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CleanTask.API.dll"]

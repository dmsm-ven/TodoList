# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8070
EXPOSE 8071


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["TodoListApp.WebUI/TodoListApp.WebUI.csproj", "TodoListApp.WebUI/"]
COPY ["TodoListApp.ApiClient/TodoListApp.ApiClient.csproj", "TodoListApp.ApiClient/"]
COPY ["TodoListApp.Core/TodoListApp.Core.csproj", "TodoListApp.Core/"]
RUN dotnet restore "./TodoListApp.WebUI/TodoListApp.WebUI.csproj"
COPY . .
WORKDIR "/src/TodoListApp.WebUI"
RUN dotnet build "./TodoListApp.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TodoListApp.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoListApp.WebUI.dll"]
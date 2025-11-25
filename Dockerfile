FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ShiftManagement.csproj", "./"]
RUN dotnet restore "ShiftManagement.csproj"

COPY . .
RUN dotnet build "ShiftManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftManagement.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

# Создаем директории для статических файлов
RUN mkdir -p /app/wwwroot/css /app/wwwroot/js

EXPOSE 8080
ENTRYPOINT ["dotnet", "ShiftManagement.dll"]

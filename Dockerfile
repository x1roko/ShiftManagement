FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ShiftManagement.csproj .
RUN dotnet restore ShiftManagement.csproj
COPY . .
RUN dotnet clean ShiftManagement.csproj
RUN rm -f *.deps.json
RUN dotnet publish ShiftManagement.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .
CMD ["dotnet", "ShiftManagement.dll"]


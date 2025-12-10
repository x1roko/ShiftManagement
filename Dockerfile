 FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
 WORKDIR /src
 COPY ShiftManagement.csproj .
 RUN dotnet restore
 COPY . .
 RUN rm -f ShiftManagement.deps.json
 RUN dotnet publish -c Release -o /publish

 FROM mcr.microsoft.com/dotnet/aspnet:8.0
 WORKDIR /app
 EXPOSE 8080
 ENV ASPNETCORE_URLS=http://+:8080
 COPY --from=build /publish .
 ENTRYPOINT ["dotnet", "ShiftManagement.dll"]


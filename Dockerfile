# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "AspWebProject.dll"]

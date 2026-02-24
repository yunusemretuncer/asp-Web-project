# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution and project files
COPY *.sln .
COPY AspWebProject/*.csproj ./AspWebProject/
COPY AspWebProject.Tests/*.csproj ./AspWebProject.Tests/

# restore
RUN dotnet restore

# copy everything else
COPY . .

# publish
RUN dotnet publish AspWebProject/AspWebProject.csproj \
    -c Release \
    -o /app/publish

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "AspWebProject.dll"]

# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solution
COPY ["Pet.ON.Api.sln", "./"]

# Copia TODOS os projetos da solution
COPY ["Pet.ON.Api/Pet.ON.Api.csproj", "Pet.ON.Api/"]
COPY ["Pet.ON.Domain/Pet.ON.Domain.csproj", "Pet.ON.Domain/"]
COPY ["Pet.ON.Infra/Pet.ON.Infra.csproj", "Pet.ON.Infra/"]
COPY ["Pet.ON.Service/Pet.ON.Service.csproj", "Pet.ON.Service/"]
COPY ["Pet.ON.Teste/Pet.ON.Teste.csproj", "Pet.ON.Teste/"]

RUN dotnet restore

# Copia todo o código
COPY . .
WORKDIR "/src/Pet.ON.Api"
RUN dotnet publish -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pet.ON.Api.dll"]
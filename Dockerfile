# Docker multi-stage: compilação do React + compilação do backend .NET + runtime.
FROM node:22-alpine AS frontend-build
WORKDIR /src/frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci
COPY frontend/ ./
# Sem VITE_API_URL no ambiente de build, o React em produção usa a mesma origem da API.
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src/backend/CatalogoCursos.Api
COPY backend/CatalogoCursos.Api/CatalogoCursos.Api.csproj ./
RUN dotnet restore
COPY backend/CatalogoCursos.Api/ ./
RUN dotnet publish -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
COPY --from=backend-build /app/publish ./
COPY --from=frontend-build /src/frontend/dist ./wwwroot
ENTRYPOINT ["dotnet", "CatalogoCursos.Api.dll"]

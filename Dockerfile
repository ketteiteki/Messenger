FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:16.18.0-alpine as reactBuild
WORKDIR /react
COPY ["Messenger.Client/package.json", "Messenger.Client/"]
COPY ["Messenger.Client/package-lock.json", "Messenger.Client/"]
COPY ["Messenger.Client/.env", "Messenger.Client/"]
WORKDIR "/react/Messenger.Client"
RUN npm ci
RUN sed -i 's|REACT_APP_BASE_API=https://localhost:7400|REACT_APP_BASE_API=http://localhost:7400|' .env
WORKDIR /react
COPY . .
WORKDIR "/react/Messenger.Client"
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src
COPY ["Messenger.Application/Messenger.Application.csproj", "Messenger.Application/"]
COPY ["Messenger.BusinessLogic/Messenger.BusinessLogic.csproj", "Messenger.BusinessLogic/"]
COPY ["Messenger.Domain/Messenger.Domain.csproj", "Messenger.Domain/"]
COPY ["Messenger.Infrastructure/Messenger.Infrastructure.csproj", "Messenger.Infrastructure/"]
COPY ["Messenger.Persistence/Messenger.Persistence.csproj", "Messenger.Persistence/"]
COPY ["Messenger.WebApi/Messenger.WebApi.csproj", "Messenger.WebApi/"]
COPY /img .
RUN dotnet restore "Messenger.WebApi/Messenger.WebApi.csproj"
COPY . .
WORKDIR "/src/Messenger.WebApi"
RUN dotnet build "Messenger.WebApi.csproj" -c Release -o /app/build --no-restore

FROM build as publish
RUN dotnet publish "Messenger.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false
COPY --from=reactBuild /react/Messenger.WebApi/wwwroot/ /app/publish/wwwroot/

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build ./src/img ./img
ENTRYPOINT [ "dotnet", "Messenger.WebApi.dll" ]
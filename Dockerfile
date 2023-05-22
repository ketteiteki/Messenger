FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build ./src/img ./img
ENTRYPOINT [ "dotnet", "Messenger.WebApi.dll" ]
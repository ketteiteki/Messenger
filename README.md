# Messenger

[![Run Build and Test](https://github.com/Ketteiteki/Messenger/actions/workflows/run-build-and-test.yml/badge.svg)](https://github.com/Ketteiteki/Messenger/actions/workflows/run-build-and-test.yml)

### To run docker container
    - `docker run --name "messenger-pgsql-db" -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:latest`

### [Demo Video](https://youtu.be/3SgCwOaLhUg)

## How to build project

### Frontend

- Install [NVM for Windows](https://github.com/coreybutler/nvm-windows)
- Validate NVM installed correctly (PowerShell as Administrator): `nvm --version`
- Install NodeJS (PowerShell as Administrator): `nvm install 18.14.0`
- Use NodeJS (PowerShell as Administrator): `nvm use 18.14.0`
- Check NodeJS installed properly: `node -v`
- Check NPM installed properly (should be 9.4.1): `npm -v`
- Install NPM v9.4.1 if necessary: `npm install -g npm@9.4.1`
- Install TypeScript globally: `npm install -g typescript@4.9.5`
- Validate TS files: `cd Messenger.Client && tsc -p tsconfig.json`
- Restore packages: `npm ci`
- Set execution policy: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Run project: `npm start`

### Backend

- Install [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Run `dotnet build`
- Run database in
  Docker: `docker run --name "messenger-pgsql-db" -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:latest`
- Install Azurite: `npm install -g azurite`
- Run Azurite: `azurite --silent --location c:\azurite --debug c:\azurite\debug.log`
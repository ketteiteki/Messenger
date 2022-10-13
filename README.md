# Messenger

[![CI](https://github.com/MessengerOrg/Messenger/actions/workflows/run-build-and-test.yml/badge.svg)](https://github.com/MessengerOrg/Messenger/actions/workflows/run-build-and-test.yml)

- To run docker container
    - `docker run --name "messenger-pgsql-db" -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:latest`
# AuctionApp

## Table of Contents

- [About](#about)
- [Requirements](#requirements)
- [Project Setup](#project-setup)
  - [Run The Client App](#run-the-client-app)
  - [Setup the API](#setup-the-api)
- [Project Structure](#project-structure)
- [Run the Client App](#run-the-client-app)
- [Usage](#usage)
  - [Client](#client)
  - [Ending the Auction](#ending-the-auction)
- [Troubleshooting](#troubleshooting)
- [What I did not do](#what-i-didnt-do)
  - [Add Toasts](#add-toasts-for-errors)
  - [Persist Messages](#persist-messages)
  - [Use Redis](#use-redis-to-store-connection-info)

## About

A case study for Lights On Heights. An auction chat application where users can join rooms, chat, and bid on auctions.

The API uses [.NET](https://dotnet.microsoft.com/en-us/learn/dotnet/what-is-dotnet), [SignalR](https://learn.microsoft.com/en-gb/aspnet/core/signalr/introduction?view=aspnetcore-8.0), [RabbitMQ](https://rabbitmq.com/) & [MassTransit](https://masstransit.io/). The database used is [Postgres](https://www.postgresql.org/). The repo includes a simple client application used to access the chat interface. The client was built with TypeScript, [Express](https://expressjs.com/) and [Bun](https://bun.sh).

If you are interested in reading the code, I suggest reading on [Vertical Slice Architecture](https://code-maze.com/vertical-slice-architecture-aspnet-core/) first. Then take a look at the configuration in Program.cs. After that, follow requests from the controller to their end. That should give you a good understanding of the application.

Also read [BehindTheScenes.md](./BehindTheScenes.md)

## Requirements

Ideally, you would need to have bun installed to use the client application. You can install it [here](https://bun.sh/docs/installation).
To the run the API, you will need to have [Docker](https://docs.docker.com/engine/install/) installed.

## Project Setup

To get started with this project, follow these steps:

Clone the repository to your local machine:

```bash
git clone https://github.com/henrychris/AuctionApp.git
```

Navigate to the project directory:

```bash
cd AuctionApp
```

### Run the Client App

1. Navigate to the client directory

   ```bash
   cd client
   ```

2. Install dependencies

   ```bash
   bun install
   ```

3. Run the client application

   ```bash
   bun start
   ```

4. You can access the client application at <http://localhost:3000>

### Setup the API

1. Setup the appsettings.json file

   A settings file was shared with you. Copy its contents and paste them into **appsettings.json** located in AuctionApp/src/AuctionApp.Host.

2. Navigate to the project root.

   ```bash
   cd ..
   ```

3. Spin up the Docker containers

   ```bash
   docker compose up
   ```

4. You can access the Swagger API documentation at <http://localhost:5000/swagger/index.html>

## Project Structure

The project uses a mix of Vertical Slice Architecture and Clean Architecture. Concerns are separated into class libraries and features are grouped together.

- **Application**
  - _Features_: the core functionality. Requests sent from controllers and their handlers.
  - _Contracts_: interfaces to be implemented in the **Infrastructure** layer.
  - _Extensions_: type, object or class extensions.
- **Common**
- **Domain**
  - _Entities_
    - _Base_: holds a base entity used by domain models
    - _Hub_: holds entities used in SignalR hubs.
    - _Notifications_: holds entities used for sending emails
  - _Enums_
  - _ServiceErrors_: errors are standardised in this project. They are defined here.
  - _Settings_: classes that are used to fetch settings defined in configuration.
- **Infrastructure**
  - _Middleware_
  - _Data_
  - _Hubs_: holds the SignalR hubs.
  - _Filters_
  - _Services_: implementations of the contracts from the **Application** layer.
- **Host**
  - _Configuration_: application configuration and setup.
  - _Controllers_
  - _Templates_: html templates used for emails.
  - Program.cs
- **Tests**

### More On Structure

- **Application**: contains the features and controllers. A feature consists of its requests, responses, controller,
  validators and handlers.
- **Common**: a set of files shared across all modules.
- **Domain**: contains core domain entities, constants, enums and business logic.
- **Infrastructure**: contains external concerns like a database or external APIs. Any interfaces defined in the
  application layer (or other inner layers), should be implemented here.
- **Host**: The entry point. This layer configures and starts the application.

## Usage

Some things have been setup to let you test the essential functions.

There are two users seeded into the DB

1. Email: <test@email.com>. Role: Admin.
2. Email: <test2@email.com>. Role: User.

The password is 'testPassword123@' for both users.

The admin user can be used to:

- create, update, get and delete auctions.
- create and open rooms.
- start and end auctions in bidding rooms.

The regular user can:

- join and leave rooms.
- bid on auctions and send messages while in a room.
- view rooms with open auctions to join.

### Client

On loading the client, you will be greeted with a login page. If you want to receive an email on conclusion of an auction, I suggest that you choose register and provide a **real** email.

After login/registration, you will see a page with a list of rooms. There's only one room available, which is the one seeded into the database. You can join the room to place bids and chat.

**Note**: At this point, you might want to open a private or incognito window and login as a different user. With the other user, you can join the same auction room and chat/bid against one another.

### Ending the Auction

When you have placed a sufficient amount of bids, you can **end** the auction. This means:

- all users will be kicked from the room
- the room will be closed and users will be unable to rejoin.
- the highest bidder will receive an invoice - if they provided a valid email address.

To end the auction, send a **POST** request on the route `api/rooms/biddingRoom1/end`. Here, **biddingRoom1** is the id of the default auction room.

## Troubleshooting

- If you forget to add appsettings before running the application, it will fail to start. Run `docker compose --build auctionapp.host` to rebuild the host image.

- Sometimes the RabbitMQ service starts slower than the others, this is normal. The API will retry until it connects to RabbitMQ.

## What I didn't do

### Add toasts for errors

I built the client app with basic HTML, CSS and TypeScript (transpiled to JS). As such, I didn't add toasts or notifications for error messages. However, all error messages are **readable**. By monitoring the browser console, you can check if anything went wrong.

### Persist messages

You'll notice that on leaving a room and rejoining, any previous messages will be lost. I did not persist messages as the aim was to demonstrate a chat application, not to build WhatsApp.

### Use Redis to store connection info

To scale when using SignalR, developers use Redis to store info on connections or groups, instead of storing that info in memory (like I did in this application). When the Redis cluster is distributed, users would still be able to access their groups despite being connected to different servers, as each server can query Redis.

### Add Comprehensive End-to-End tests

For testing websockets connections, chat, etc. This would take a lot of time to setup. I did not have the time.

## Links

You can access the Postman Collection [here](https://documenter.getpostman.com/view/22039666/2s9YythM1s).

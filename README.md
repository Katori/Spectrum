# Notice: What is Spectrum Today?

Below is an aspirational target for Spectrum 0.1. Currently Spectrum 0.1 is in development. Look (and use) at your own risk.

# Spectrum

## What Is Spectrum

Spectrum is a solution for deploying your Mirror games (or, with little modification, your UNET/HLAPI games), and running a truly painless in-house multiplayer solution with no caps, CCUs, or the like. You can run it on your local machine for development all the way up to a VPS for production scale.

## What Does Spectrum Require

Let's get this out of the way first. Spectrum has ONLY been tested on Unity 2018.2.14 using IL2CPP and .NET Standard 2.0. It is the author's belief that this target is the best way to develop Unity games in late 2018. In fact, it's only been tested on Windows, but there's no reason why it won't work elsewhere, and further testing on other platforms is coming (and welcome if anyone wants to assist).

## How Does Spectrum Work

Inspired by Master Server Framework but massively simplified and done "the Mirror way", Spectrum uses a Master-Spawner-Server-Client architecture.

### Master Server

- Connects to Spawners, Game Servers, and Clients.
- Responsible for:
  - Telling Spawners when to to spawn servers on what port
  - Registering servers as possible connections for clients
  - Keeping track of how many clients are connected to each server
  - Telling clients what servers to connect to

### Spawner Server

- Connects to Master Server
- Responsible for:
  - Spawning Game Servers

### Game Server

- Connects to Master Server
- Responsible for:
  - All game logic - what we would traditionally let UNET and now let Mirror handle
  - Initiating and maintaining a constant connection between itself and the Master Server
  - Connecting to clients, reporting these connections (and their associated disconnections) to the Master Server for capacity measurement purposes

### Game Client

- Connects to Master Server and Game Server
- Initially connects to Master Server to receive Game Server information, then terminates that connection and connects to Game Server.
- Shares a NetworkManager and much logic with Game Server - simple as we can get.

## Why Do Things This Way

This way, we can achieve the following setup:

- The Master Server runs on a dedicated instance in Headless Mode.
- The Spawner Servers (as many as you desire, their capacity can be adjusted effortlessly) run each on their own dedicated instance in Headless Mode.
- The Game Servers (as many as you desire per Spawner) share an instance with their Spawner, but each run on a different port, in Headless Mode. A special MonoBehaviour is provided to enhance Headless Mode by disabling all renderers and any desired GameObjects.
- The Client connects effortlessly to the Game Server through the Master Server and is none the wiser. In Graphics Mode.

Of course, you are free to adjust the specifics of instancing all you like--these variables are easy to change, and all code in the project is targeted at a standard Unity binary that can be run on Windows, macOS or Linux. You could have the Master, Spawner and Game Servers all run on the same instance if you like--nothing is stopping you but the configuration of ports and the hardware necessary to achieve performance with such a setup.

## Goodies

Spectrum also provides a First-Person example using Mirror.

## Roadmap

- Advanced authentication
- Load balancing
- Master data transfer (optional client persistent connection to Master Server)
  - Optional SQLite module for Master Server
  - Optional chat module for Master Server and client
- Battle-proven WebGL support (WebGL not currently tested)
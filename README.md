# Lantern

<p align="center">
    <img src="https://github.com/LanternBE/Lantern/blob/main/Assets/Banner.png" alt="Image"/>
</p>

> [!WARNING]
> Currently under active development and in its early stages. 🚧

A Minecraft: Bedrock Edition server software built in C#, designed to be lightweight, modular, and extensible.

[![Minecraft - Version](https://img.shields.io/badge/minecraft-v1.21.80_(Bedrock)-black)](https://feedback.minecraft.net/hc/en-us/sections/360001186971-Release-Changelogs)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Last Commit](https://img.shields.io/github/last-commit/LanternBE/Lantern?color=blue)](https://github.com/LanternBE/Lantern/commits/Lantern)
[![Stars](https://img.shields.io/github/stars/LanternBE/Lantern?style=social&color=blue)](https://github.com/LanternBE/Lantern/stargazers)

## Getting Started

First of all you need to open a terminal in your IDE, and clone the repo with:
```bash
git clone https://github.com/LanternBE/Lantern.git
```

After that, just run the Program.cs file, or type this command in the terminal:
```bash
dotnet run
```

# Features & ToDo

| Elements                            | Status |
|-------------------------------------|--------|
| RakNet Protocol (Server & Client)   |   ✅   |
| Bedrock Protocol (Server & Client)  |   🚧   |
| Encryption                          |   ❌   |
| Console Commands                    |   ❌   |
| Plugins Support                     |   ❌   |
| Worlds Support (.mcworld & LevelDb) |   ❌   |

# References

This software was born mainly thanks to these repos that we based ourselves on, taking inspiration from:

- [PocketMine-MP](https://github.com/pmmp/PocketMine-MP) - RakNet & Bedrock Protocol
- [DaemonMC](https://github.com/laz1444/DaemonMC) - RakNet & Bedrock Protocol
- [gophertunnel](https://github.com/Sandertv/gophertunnel) - Bedrock Protocol
- [PieMC](https://github.com/PieMC-Dev/PieMC) - ReadMe
- [EndStone](https://github.com/EndstoneMC/endstone) - ReadMe

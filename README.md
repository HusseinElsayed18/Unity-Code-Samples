# Unity C# Code Samples

This repository contains selected C# code samples from my Unity game development projects. The samples demonstrate practical approaches to building modular, maintainable, and reusable game systems.

Code Samples
1. CustomizationControlPopup

An interactive UI customization system that allows players to personalize their in-game controls.

Key concepts:

Dynamic UI control customization.
Position, scale, and transparency adjustment.
Screen and layout presets.
Save and reset functionality.
Event-driven programming.
Service-based architecture.
2. AdsInstaller

A dependency injection installer built with Zenject to configure and register multiple advertising providers.

Key concepts:

Dependency injection using Zenject.
Interface-based service registration.
Multiple advertising provider support.
ID-based bindings to distinguish implementations.
Centralized service configuration.
Separation of advertising integration and game-level advertising logic.

The installer registers:

AdsService for Unity Ads.
LevelPlayAdsService for LevelPlay Ads.
GameAdsService as the main game-level advertising service.
Project Structure

```text
Assets/
└── Scripts/
    ├── CustomizationControlPopup.cs
    └── AdsInstaller.cs
```

Technologies & Concepts
Unity Engine
C#
Zenject (Dependency Injection)
Object-Oriented Programming
Interface-Based Design
Event-Driven Architecture
Modular Game Systems

Note: These are selected code samples extracted from larger Unity projects. Some referenced classes, interfaces, and configuration objects belong to their respective project architectures and are not included as standalone implementations.

<p align="center">
  <a href="https://husseinelsayed18.github.io">🌐 Portfolio</a> &nbsp;&bull;&nbsp;
  <a href="https://www.youtube.com/@egygamesstudios">📺 Egy Games Studios YouTube</a> &nbsp;&bull;&nbsp;
  <a href="mailto:husseinelsayed4g@gmail.com">📧 Email Me</a>
</p>

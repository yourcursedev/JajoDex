<div align="center">

<!-- შეგიძლია შენი ლოგოს ფაილი ატვირთო რეპოზიტორიაში და ქვედა ხაზი ჩართო -->
<!-- <img src="Assets/logo.png" alt="JajoDex Logo" width="110" /> -->

# JajoDex

**Ultra-Lightweight External Crosshair Overlay Studio for Windows**

[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-09090B?style=for-the-badge&logo=windows&logoColor=9D4EDD)](https://github.com/yourcursedev/JajoDex/releases)
[![Framework](https://img.shields.io/badge/.NET-10%20WPF-09090B?style=for-the-badge&logo=dotnet&logoColor=9D4EDD)](https://dotnet.microsoft.com/)
[![Anti-Cheat](https://img.shields.io/badge/Anti--Cheat-Safe%20(External)-09090B?style=for-the-badge&logo=shield&logoColor=10B981)](#anti-cheat-safety)
[![Release](https://img.shields.io/badge/Download-v1.0.0-7B2CBF?style=for-the-badge)](https://github.com/yourcursedev/JajoDex/releases/latest)

*Designed for Windowed & Borderless Windowed Fullscreen games — zero injection, sub-pixel precision, animated crosshairs, and an in-game radial quick-switch wheel.*

</div>

---

<!-- სქრინშოტი რომ დაამატო, ატვირთე სურათი რეპოში (მაგ: preview.png) და ჩასვი ქვემოთ -->
<!-- ![JajoDex Studio Preview](Assets/preview.png) -->

## Features

- **Sub-Pixel Crosshair Engine** — Independent geometry controls for **Center Dot**, **Inner Lines**, and **Outer Lines** (length, thickness, gap, opacity, and per-arm Top/Bottom/Left/Right toggles).
- **Effects & Color Studio** — Outlines, glow/drop shadow, rotation, master scale, interactive HSV/Hex Color Picker with esports presets, and a live zoomable preview canvas.
- **Animated Crosshairs & Custom Media** — Built-in **RGB Rainbow Cycle**, **Breathing/Pulse**, and **Continuous Spin** animations, plus custom **`.png`**, **`.webp`**, **`.svg`**, and **animated `.gif`** import.
- **In-Game Radial Wheel** — Press `Left Ctrl + Left Alt` to open a circular glassmorphic wheel with live miniature previews of all your saved profiles. Hover and click to switch instantly.
- **Per-Game Auto-Switching** — Link profiles to game executables (e.g., `cs2.exe`, `VALORANT-Win64-Shipping.exe`) for instant automatic switching when the game gains focus.
- **Dynamic Hooks** — Optional **ADS Mode** (hide on right-click) and **Firing Error** (subtle expansion while shooting).
- **Share & Backup** — Save unlimited profiles locally, export/import `.json` files, or share crosshairs instantly with compact **Base64 Share Codes**.
- **Custom Dark Glassmorphic Studio** — Hover-expandable icon rail, custom system tray menu, multi-monitor DPI V2 support, and a dedicated custom installer & uninstaller.

---

## Anti-Cheat Safety

JajoDex is **100% external**:
- Uses standard Windows layered click-through windows (`WS_EX_LAYERED`, `WS_EX_TRANSPARENT`).
- **No** DLL injection, **no** DirectX hooking, and **no** game memory reading or writing.
- Runs at **<0.5% idle CPU** with hardware-accelerated vector rendering.

---

## Default Hotkeys

| Action | Default Shortcut |
| :--- | :--- |
| Open Radial Profile Wheel | `Left Ctrl + Left Alt` |
| Toggle Crosshair Visibility | `Alt + X` |
| Show / Hide JajoDex Studio | `Alt + M` |
| Cycle Profiles | `Alt + Left` / `Alt + Right` |
| Nudge Position (1px) | `Ctrl + Alt + Arrow Keys` |

*All hotkeys can be rebound inside the **Settings** tab.*

---

## Installation

1. Go to the **[Latest Release](https://github.com/yourcursedev/JajoDex/releases/latest)**.
2. Download **`JajoDex_Setup_v1.0.0.exe`**.
3. Run the custom installer (no separate .NET runtime needed).
4. Set your game to **Windowed** or **Borderless Windowed Fullscreen** and launch JajoDex.

---

## Tech Stack

- **C# / WPF (.NET 10 LTS)** — MVVM architecture (`CommunityToolkit.Mvvm`)
- **Win32 Interop** — Layered click-through overlay, DWM dark glass backdrop, `SetWinEventHook` foreground process detection, and global hotkeys.

---

<div align="center">
Made by <a href="https://github.com/yourcursedev"><strong>yourcursedev</strong></a>
</div>
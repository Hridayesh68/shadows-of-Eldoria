# Shadows of Eldoria ⚔️🛡️

**Shadows of Eldoria** is a top-down **2D Action Roguelike** developed in **Unity 6 (6000.3.11f1)**. Set in a vibrant yet perilous pixel-art realm, players take on the role of an armored knight navigating multi-tiered terrain, battling goblin hordes, managing real-time combat physics, and upgrading character attributes through an interactive in-game stat system.

---

## 📖 Table of Contents
* [Gameplay & Core Mechanics](#-gameplay--core-mechanics)
* [Technical Stack & Engine Details](#️-technical-stack--engine-details)
* [Project Architecture & Scripts](#-project-architecture--scripts)
* [Scene Hierarchy Overview](#-scene-hierarchy-overview)
* [Asset Structure](#-asset-structure)
* [Getting Started](#-getting-started)
* [Controls](#-controls)
* [Development Roadmap](#️-development-roadmap)
* [Contributing](#-contributing)
* [License](#-license)

---

## 🎮 Gameplay & Core Mechanics

* **Top-Down 2D Action Combat:** Engage in fast-paced melee encounters featuring responsive hitboxes, attack cooldowns, and dynamic **knockback physics** that give weight and impact to every strike.
* **Multi-Tiered Elevation System:** Traverse elevated cliffs, ramps, and bridges seamlessly. The custom elevation architecture dynamically adjusts sorting layers and collision bounds as the player moves between ground level and high ground.
* **Dynamic Enemy AI:** Face off against enemies like the **Goblin (`Goblin_enemy1`)**, equipped with autonomous tracking, patrol/chase state logic, melee combat routines, and health/knockback response systems.
* **Character Stats & Progression UI:** Monitor vitality in real time via a parchment-styled HUD (`Health: 100/100`) and open the interactive **STATS** panel (`Stats_Panel`) to view and allocate character attributes mid-run.
* **2D Lighting & Atmosphere:** Built using Unity's **Universal Render Pipeline (URP) 2D** with `Global Light 2D` for rich environmental illumination across handcrafted tilemaps.

---

## 🛠️ Technical Stack & Engine Details

* **Game Engine:** [Unity 6.3 LTS (`6000.3.11f1`)](https://unity.com/)[cite: 2]
* **Graphics API:** DirectX 12 (`<DX12>`)
* **Rendering Pipeline:** Universal Render Pipeline (URP) with 2D Lighting (`Global Light 2D`)
* **Programming Language:** C#
* **Version Control:** Git / Unity Version Control (`main` branch)[cite: 1, 2]
* **IDE:** Visual Studio Code

---

## 🧩 Project Architecture & Scripts

The codebase is modularized into single-responsibility C# scripts located under `Assets/Scripts`[cite: 2]:

### Player Systems
* **`PlayerMovement.cs`**: Handles 8-directional / top-down 2D movement, velocity smoothing, sprite flipping, and animator state transitions[cite: 2].
* **`Player_Combat.cs`**: Manages melee attack inputs, weapon hitbox activation, attack timing, and damage delivery to enemies[cite: 2].
* **`PlayerHealth.cs`**: Tracks player vitality (`100/100` base), processes incoming damage, updates the on-screen Health UI banner, and handles defeat states[cite: 2].

### Enemy AI & Physics
* **`Enemy_Movement.cs`**: Controls enemy pathing, player detection ranges, and pursuit behavior[cite: 2].
* **`Enemy_Combat.cs`**: Executes enemy attack patterns, damage frames, and cooldowns when in range of the player[cite: 2].
* **`EnemyHealth.cs`**: Manages individual enemy HP pools, damage calculation, and death triggers[cite: 2].
* **`EnemyKnockback.cs`**: Applies physics-based impulse forces upon taking damage, pushing enemies backward and briefly interrupting their movement for tactile combat feedback[cite: 2].

### World & Elevation Mechanics
* **`Elevation.cs`**: Triggered when entering elevated terrain zones (cliffs, raised platforms); updates the player's sorting order and physics layer collisions so characters properly render above lower terrain[cite: 2].
* **`Elevation_Exit.cs`**: Resets sorting layers and collision masks when stepping down from elevated zones back onto base terrain[cite: 2].

### UI & Game Management
* **`StatManager` & `Stats_Panel`**: Coordinates persistent run statistics and controls the interactive parchment UI overlay (`STATS`) for viewing and modifying player attributes[cite: 2].

---

## 🌳 Scene Hierarchy Overview

The primary playable environment (`SampleScene`) is structured cleanly for scalability[cite: 2]:

```text
SampleScene
├── Global Light 2D        # Scene-wide 2D URP illumination
├── Player                 # Main playable knight character with combat & health components
├── Elevation Base         # Tilemap/collider architecture for multi-level terrain
├── water                  # Water hazard and boundary tilemap layer
├── Cameras                # Cinemachine / Main Camera tracking setup
├── Goblin_enemy1          # AI enemy prefab instance
├── Canvas                 # Screen-space UI container (Health banner & overlays)
├── EventSystem            # UI input and button interaction handler
├── StatManager            # Core controller for player attributes and progression
└── Stats_Panel            # Toggleable parchment UI window for character stats

Assets/
├── Prefab/                # Reusable game objects (Enemies, Projectiles, UI elements)
├── Scenes/                # Game scenes (SampleScene)
├── Scripts/               # Modular C# gameplay, AI, and elevation scripts
├── Settings/              # URP 2D renderer and project configuration assets
└── Sprites/               # Handcrafted 2D pixel art assets
    ├── Canvas/            # Parchment banners, buttons, and stat panels
    ├── characters/        # Knight and Goblin character sprite sheets & animations
    └── terrain/           # Environmental tile palettes
        ├── Decorations/   # Trees, foliage, and props
        ├── Elevation/     # Cliff walls, ledges, and stairs
        ├── grass tiles 1/ # Ground textures and variations
        ├── roads/         # Dirt paths and walkways
        └── rocks/         # Obstacles and environmental details




# CLAUDE.md: Shadows of Eldoria

Guidance for Claude Code when working in this repository. Read this file fully at the start of every session.

@PROGRESS.md

## 1. Project Summary

**Title:** Shadows of Eldoria
**Project path:** `/data/GameDev Unity/shadows-of-Eldoria` (the path contains a space, so always quote it in shell commands)
**Genre:** Isometric 2D medieval action roguelike, inspired by *Hades* (fast, responsive combat; room-by-room runs; boon drafting).
**Art direction:** Pixel art with strong readability (Tiny Swords-style medieval look). Use only the sprite packs already in `Assets/`.
**Engine:** Unity **6000.6.10f1** (Unity 6). Use Unity 6 APIs (e.g. `Rigidbody2D.linearVelocity`, not `velocity`). Check current Unity 6 docs before using any API you are unsure about.

### Core Loop
1. **Hub / Class Select (UI):** Player picks a class. Each class has a primary attack, a movement/dash ability, and a starting weapon aspect.
2. **Timed Dungeon Floor:** Player clears an isometric room of monsters (skeletons, raiders, elites) against a floor timer.
3. **Clear Rewards:** Clearing within the target time grants a currency multiplier and a bonus reward. Going over time triggers a **Corrupted Surge** (enraged enemies or extra elite spawns). The timer must never cause an instant game-over.
4. **Reward Choice:** After a clear, 2-3 exit portals/altars each show the reward type for the next room (Weapon Tempering, Class Boon, Relic/Gold, Heal/Sanctuary). The player picks one, then descends to a harder floor.
5. **Death & Meta-Progression:** Run Boons reset on death. Soul Shards persist and are spent in the hub on permanent unlocks (new classes, weapon aspects, deeper dungeon tiers).

### Starter Classes
| Class | Weapon | Identity | Scales with |
|---|---|---|---|
| **Vanguard** | Sword & Shield | Cleaving melee arcs, dash-bash stun, high armor/poise | Cleave size, bleed, thorns |
| **Shadowblade** | Dual Daggers | Fast multi-hit combos, backstab/crit, short blink | Crit chains, poison, attack speed |
| **Arcanist** | Staff / Runes | Ranged elemental bolts, zone sigils, mana shield | Chain lightning, burn, AoE radius, cooldown reset |

### Pacing Targets (tunable, keep in ScriptableObjects, never hardcode)
- Target clear time per floor: 60-90 seconds.
- Run Boons are temporary (e.g. "Dash leaves a fire trail", "Every 4th hit crits").

---

## 2. Project Configuration & Status

Detected from the project files. Update this section whenever a decision changes. **Do not assume answers to open items; inspect the project or ask.**

### Confirmed
- **Unity version:** 6000.6.10f1.
- **Render pipeline:** URP 17.6.0 (`com.unity.render-pipelines.universal`) with a **2D Renderer** (`Assets/Settings/Renderer2D.asset`).
  - The Graphics setting has no pipeline assigned; a URP asset is assigned in **every Quality level** instead (GUID `681886c5eb7344803b6206f758bf0b1c`). If you change the pipeline asset, change it in Quality settings for all levels.
  - Use URP 2D features: `Light2D`, 2D lit sprite materials (`Sprite-Lit-Default`), Shadow Caster 2D. Do not use Built-in or 3D URP shaders for sprites.
- **Input:** Input System package 1.20.1, with `activeInputHandler: 2` (both Input System and legacy Input Manager enabled).
  - Write all new input code with the **Input System package** (an `InputActionAsset` in `Assets/_Game/`). Do not use `UnityEngine.Input` (legacy).
  - Suggest switching Active Input Handling to "Input System Package (New)" once nothing legacy remains. Do not change it without asking.
- **2D packages installed:** `2d.animation` 16.0.1, `2d.aseprite` 6.0.0, `2d.psdimporter` 15.0.1, `2d.sprite`, `2d.spriteshape` 16.0.0, `2d.tilemap`, `2d.tilemap.extras` 9.0.1 (provides Isometric Tile / Rule Tile types), `2d.tooling` 4.0.1.
- **UI framework:** **uGUI (Canvas + EventSystem) with TextMeshPro.**
  - No UI Toolkit assets exist in the project (no `.uxml`, `.uss`, or `UIDocument` usage).
  - A Canvas and EventSystem currently exist only in `Assets/Scenes/SampleScene.unity`. No project-owned UI scripts exist yet.
  - TextMesh Pro example scenes/scripts under `Assets/TextMesh Pro/` are package samples. Never use them as game content or edit them.
  - Do not introduce UI Toolkit without asking the user.

### Open
- [ ] Grid cell layout: **Isometric** vs **Isometric Z as Y** (choose once, record here).
- [ ] Which sprite packs in `Assets/` are already extracted and usable (inspect before Milestone 1; see section 3).
- [ ] Final game title/branding for the main menu (working title: Shadows of Eldoria).

### Current milestone
**Milestone 1** (see section 6). `Assets/Scenes/SampleScene.unity` is the template scene; do not build game content in it. Create scenes under `Assets/_Game/Scenes/`.

---

## 3. Folder & Asset Rules

`Assets/` already contains imported third-party packs (tiny/medieval characters, skeleton and monster packs, example terrain) and some `.zip` archives.

- **Never** add project-owned code, prefabs, scenes, ScriptableObjects, or tile palettes inside third-party pack folders.
- **Never** reference files inside `.zip` archives from scenes or prefabs. Use only sprites/animations/tiles already extracted in `Assets/`.
- **Inspect existing folders before importing or duplicating any sprite.** Reuse what exists.
- Do not move, rename, or delete third-party asset folders (it breaks GUID references). Reference them in place.
- Always keep `.meta` files alongside their assets. Never hand-edit `.meta` GUIDs.

All project-owned content lives under `Assets/_Game/`:

```
Assets/_Game/
  Scripts/        Core/ Player/ Classes/ Combat/ Enemies/ Dungeon/ Upgrades/ UI/
  Prefabs/        Player/ Enemies/ Projectiles/ Interactables/ UI/
  ScriptableObjects/  Classes/ Weapons/ Boons/ Enemies/ Dungeons/
  Scenes/         MainMenu, Hub, Dungeon_01, ...
  Tiles/          Isometric Tile Palettes and Rule Tiles built from imported terrain sprites
```

---

## 4. Technical Standards

### Isometric 2D
- Use a 2D `Grid` with **Isometric** or **Isometric Z as Y** cell layout. Pick one and use it everywhere. Record the choice in section 2.
- Tilemap layers: `Ground`, `Walls`, `Decor`, `Collision`.
- Sprite sorting: set the Transparency Sort Mode to **Custom Axis** in Graphics/Camera settings. For Isometric Z as Y, Unity's documented axis is `(0, 1, -0.26)`; for standard Isometric use the Y-based axis. Verify against the Unity version's docs. Use `SortingGroup` on multi-sprite characters. Characters, enemies, walls, and props must not flicker or sort incorrectly.
- Movement: 8-directional input converted to isometric space (movement direction vectors must be transformed so "up" on screen moves along the isometric axis correctly). Use `Rigidbody2D` with Gravity Scale 0 and Interpolate on. Use `linearVelocity` (Unity 6). Read movement from the Input System package (see section 2).
- Physics: keep collision on dedicated layers (Player, Enemy, PlayerHitbox, EnemyHitbox, Environment, Projectile) and configure the Layer Collision Matrix deliberately.

### Data-Driven Design
Define these as `ScriptableObject` assets (no hardcoded switch statements for content):
- Classes, Weapons, Weapon Upgrades, Boons, Enemy Archetypes, Dungeon Floors (including target clear time and surge rules).

Adding a new boon, enemy, or class should require only a new asset plus (at most) a small new behavior class, not edits across many files.

### Combat
- Shared `IDamageable` and `HealthComponent` used by player, enemies, and bosses.
- `Hitbox` / `Hurtbox` components for damage delivery. Support damage types and status effects (bleed, poison, burn) through data, not special cases.

### UI & Decoupling
- HUD and menus (class select, floor timer, XP/level bar, 3-card boon/upgrade draft) are **event-driven** (C# events or ScriptableObject event channels).
- Gameplay code must **never** reference UI MonoBehaviours directly. UI subscribes to gameplay events, not the reverse.

### Code Style
- C# namespaces mirror folders (e.g. `Game.Combat`).
- One MonoBehaviour per file; filename matches class name.
- Prefer `[SerializeField] private` over public fields.
- Avoid `Find*`/`GetComponent` in `Update`; cache references.
- No magic numbers for gameplay values; expose them via ScriptableObjects or serialized fields.
- Add short comments only where intent is non-obvious.

---

## 5. Unity CLI & MCP Workflow

> Command names below reflect the intended tooling setup. If a command is not found or behaves differently, run its `--help`, tell the user, and adapt rather than guessing.

1. **Verify the target first.** At session start and before mutating scenes/assets, run `unity status` and confirm the connected Editor matches this project directory (`/data/GameDev Unity/shadows-of-Eldoria`, Unity 6000.6.10f1, port 7800 at last check). If it does not match, stop and tell the user. The path contains a space: always quote paths in shell commands.
2. **Inspect before building.** Read `Packages/manifest.json` and list relevant `Assets/` folders before creating new systems or UI.
3. **Tool division:**
   - **Shell/file tools:** write and refactor C# under `Assets/_Game/Scripts/`.
   - **`unity recompile`:** run immediately after any C# change and read the console output. Fix all compile errors **before** touching scenes or prefabs.
   - **`unity-editor-mcp`:** live scene hierarchy inspection, Tilemap/Grid setup, prefab assembly, assigning ScriptableObject references, reading console logs.
4. **Verify your work.** After scene/prefab changes, check the console for errors/warnings and, where possible, enter Play Mode to confirm behavior. Do not report a task done on compile success alone.
5. **Scene safety.** Prefer small, targeted scene edits. Do not rewrite or reserialize whole scenes/prefabs by hand. Save scenes after MCP edits.
6. **Version control.** If the project is a git repo, make focused commits per completed step. Never commit `Library/`, `Temp/`, `Logs/`, or `UserSettings/`.

### Git & GitHub Workflow
- SESSION START
  - Run `git status` and `git branch --show-current` at the start of each session.
  - If this folder is not a git repo, ask my permission before `git init`. After I approve, create the Unity .gitignore first, then make the initial commit only with my approval.
- UNITY REPO HYGIENE
  - .gitignore must ignore: Library/, Temp/, Obj/, Logs/, UserSettings/, Build/, Builds/, .vs/, .idea/, .vscode/, and generated *.csproj / *.sln files.
  - Always commit Assets/, Packages/ (manifest.json and packages-lock.json), ProjectSettings/, and every .meta file. A .meta file goes in the same commit as its asset.
  - Confirm Project Settings > Editor has Asset Serialization = Force Text and Version Control Mode = Visible Meta Files. Warn me if not.
  - If the repo would contain large binaries (roughly over 50 MB total, or many large PSD/PNG/audio files), tell me and suggest Git LFS before committing them. Do not set up LFS or rewrite history without asking.
  - Never commit secrets, API keys, tokens, or personal files.
- WHEN TO COMMIT (ASK EVERY TIME)
  - After every few related changes that form a coherent, working unit (e.g. a script plus its prefab/scene wiring, or one completed milestone step), propose a commit.
  - Only propose a commit when `unity recompile` is clean and the console has no errors. No commits of broken or half-finished work, unless I ask for a clearly labeled `wip:` commit.
  - Ask my permission before EVERY `git commit`. Show the files changed (`git status` / `git diff --stat`) and a proposed commit message, then wait for approval. An earlier approval does not cover later commits.
  - Keep commits focused: one logical change each. Do not mix third-party asset imports, scene changes, and script changes in one commit when they can be separated.
  - Message style: imperative with a prefix, e.g. `feat: add isometric player movement`, `fix: correct sprite sorting on walls`, `chore: add Unity .gitignore`, `assets: import skeleton sprite pack`.
  - Stage files explicitly. Do not use `git add -A` blindly; review `git status` first.
- BRANCHES
  - Suggest one branch per milestone or feature (e.g. `milestone-1-iso-room`) and ask before creating or switching branches. Keep `main` working.
- GITHUB / REMOTE
  - Ask before adding a remote, creating a GitHub repo, or running `git push`. If the `gh` CLI is installed, offer to use it to create the repo and ask whether it should be private.
  - Never push, force-push, rebase published history, amend pushed commits, or run destructive commands (`git reset --hard`, `git clean -fd`, `git checkout .`) without my explicit permission for that specific command.
  - After I approve a push, tell me which branch and remote were pushed.
- SAFETY WITH UNITY OPEN
  - Before `git checkout`, `git switch`, `git stash`, or `git pull`, ask me to save open scenes. Afterward run `unity recompile` and check the console.
  - On merge conflicts in .unity or .prefab files, stop and ask me. Do not resolve scene/prefab YAML conflicts by hand without permission.

### Shell Command Rules
- The session's working directory is already the project root. Never prefix commands with `cd` and never chain commands with `&&` or `;`. Run one simple command per call.
- Use the Read, Grep, and Glob tools for reading and searching files instead of `cat`, `grep`, or `find` in the shell.
- Always quote paths, because the project path contains a space.
- Prefer read-only commands for inspection. If a command is blocked, do not retry it in a different disguise. Tell me what was blocked and why you needed it.

---

## 6. Milestones (build in order; finish and verify each before the next)

1. **Isometric test room:** Tilemap room using existing terrain sprites, player movement + dash, correct sprite Y-sorting.
2. **Classes & combat:** Class select UI, 3 starter classes, `Hitbox`/`HealthComponent`, one skeleton enemy type with basic AI.
3. **Floor loop:** Floor timer, room-clear detection, Corrupted Surge, XP/level-up, 3-card Boon/Weapon upgrade draft UI.
4. **Progression:** Multi-floor dungeon with reward portals, enemy variety, persistent meta-progression (Soul Shards, hub unlocks).

Each milestone is "done" only when: it compiles with zero errors, the console is clean in Play Mode, and the user can play the slice.

---

## 7. Working Agreements

- At the start of every session, read PROGRESS.md and the last few DEVLOG.md entries before doing anything else, then tell me where we left off.
- Ask before making large architectural changes or adding packages.
- When a requirement is ambiguous or conflicts with this file, ask rather than assume.
- Keep this file current: when a decision is made (render pipeline, UI framework, grid mode), update section 2.
- Prefer the simplest implementation that fits the data-driven design. Do not over-engineer systems that are not needed by the current milestone.

---

## Execution & Permission Rules
- **Act, don't narrate:** When a task is clear, call the tool in the same turn instead of describing what you plan to do.
- **Verify before referencing:** Never guess or invent file paths or GameObject names. Run `find` or `unity-editor-mcp` scene/hierarchy inspection first, and only read or modify verified paths.
- **Zero-retry on permission blocks:** If any tool or shell command is blocked or denied, stop immediately, state in one sentence which command was blocked and why you needed it, and wait for user approval. Never repeat the same blocked command or restate the same plan.
- **Anti-loop guard:** Never repeat the same paragraph, list, or tool sequence twice. If stuck, stop and ask for direction.
- **Single-step execution:** Complete one concrete step at a time, report exact created/modified paths and `unity recompile` status, then stop and wait.
- **Never Overwrite Existing Scenes/Prefabs:** Never call scene-create or prefab-create commands on an existing `.unity` or `.prefab` path. Always check with `Glob` first; only modify existing scenes incrementally via MCP or ask explicit permission before replacing any `.unity` or `.prefab` file.
- **Never Run Destructive Shell Commands:** Never run `rm`, `rm -rf`, `git clean`, or `git reset --hard`.
- **Check Git History Before Claiming Data Loss:** Always check `git log --all -- "**/MainGame.unity"` before assuming a file is untracked or lost.

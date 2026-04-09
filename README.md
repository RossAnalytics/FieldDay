# FieldDay

**FieldDay** is a Wii Sports-inspired online multiplayer sports anthology built in Unity 2022 LTS with Photon Fusion 2.

Teams of 2–6 players compete across 6 mini-game modes in a playlist-style rotation.

---

## 🏅 Game Modes (Build Order)

1. **Darts** — 301/501 scoring, power/angle aiming mechanic with a moving reticle
2. **Bowling** — 10-frame scoring with spares and strikes
3. **Golf** — Par-based hole scoring, multi-shot rounds
4. **Basketball** — Timed shooting rounds, team points
5. **Soccer** — Penalty-kick style or open play goals
6. **Billiards** — 8-ball or 9-ball rules

---

## 🚀 Setup

### Requirements
- Unity **2022.3.42f1** (2022 LTS)
- Photon Fusion 2 SDK from [dashboard.photonengine.com](https://dashboard.photonengine.com)

### Steps
1. Clone this repo.
2. Open in Unity 2022.3.42f1.
3. Go to [dashboard.photonengine.com](https://dashboard.photonengine.com), create a Fusion 2 app, and copy the **App ID**.
4. In Unity: `Window → Fusion → Realtime Settings` — paste the App ID.
5. Install Photon Fusion 2 via the Package Manager or import the `.unitypackage`.

---

## 📁 Project Structure

```
Assets/Scripts/
  Core/          — GameManager, PhotonManager, LobbyManager, TurnManager, etc.
  Modes/         — One folder per sport (Darts, Bowling, Golf, Basketball, Soccer, Billiards)
  UI/            — Lobby, HUD, Scoreboard
  Shared/        — Shared utilities (PhysicsHelper, etc.)
Packages/        — manifest.json (Unity packages)
ProjectSettings/ — ProjectVersion.txt
```

---

## ➕ Adding a New Game Mode

1. Create a folder under `Assets/Scripts/Modes/YourMode/`.
2. Create `YourModeGameMode.cs` that extends `GameModeBase`.
3. Implement all abstract methods: `StartMode`, `EndMode`, `OnTurnStart`, `OnTurnEnd`, `CalculateWinner`.
4. Register the mode in `GameModePlaylist.cs`.

---

## 🗺 Build Roadmap

- [ ] Scene setup for each mode (art, physics colliders)
- [ ] Photon Fusion 2 App ID wired up
- [ ] Lobby → Mode selection → Playlist rotation
- [ ] Full Darts implementation (aiming + 301/501 scoring)
- [ ] Full Bowling implementation (10-frame + pin physics)
- [ ] Golf, Basketball, Soccer, Billiards stubs → full implementation
- [ ] UI polish (HUD, Scoreboard, Lobby)
- [ ] Audio + VFX pass
- [ ] Platform build (PC / WebGL)

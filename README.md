# Jackpot Slots

A compact three-reel slot machine built in Unity 6 for the Slot Game Assignment. Every reel outcome is generated independently, and a win occurs only when all three symbols match.

## Play

1. Open `Build/WebGL/index.html` through a local web server. Browsers block some WebGL files when opened directly from disk.
2. From the repository root, a quick option is `python -m http.server 8080 --directory Build/WebGL`.
3. Visit `http://localhost:8080` and press **SPIN**. Each spin costs 10 credits.

To play in the editor, open the project in Unity `6000.4.9f1`, open `Assets/Scenes/SampleScene.unity`, and enter Play mode.

## Rules and payouts

| Three matching symbols | Reel weight | Payout |
| --- | ---: | ---: |
| Seven | 2 / 20 (10%) | 1000 credits |
| Bar | 3 / 20 (15%) | 400 credits |
| Bell | 5 / 20 (25%) | 200 credits |
| Cherries | 10 / 20 (50%) | 33 credits |

The player starts with 100 credits. **RESET CREDITS** restores the starting balance when needed.

## Approach

- `SlotGameModel` contains wallet, win, and payout rules without any UI dependencies.
- `SlotRandom` owns a session-seeded PRNG. Each reel independently selects one of 20 virtual stops. The complete result is generated before animation starts, so frame rate, previous spins, and reel timing cannot influence outcomes.
- `ReelView` handles smooth acceleration/deceleration, symbol cycling, staggered stops, and a landing bounce.
- `SlotGameController` is the composition root and builds a responsive 1920×1080 canvas from the provided artwork at runtime.
- `SlotBuildTools` provides **Jackpot Slots → Build WebGL** and a batch-mode build entry point.

## Bonus polish

- Animated physical lever using the supplied machine frames.
- Staggered reel stops, squash-and-stretch landing, jackpot modal, paytable, wallet feedback, and responsive Canvas scaling.
- Lightweight Edit Mode tests for the model and payout table.

## Project structure

```text
Assets/
  Editor/               WebGL build tooling
  Resources/SlotArt/    Imported assignment artwork
  Scripts/Core/         Rules, outcomes, and RNG
  Scripts/UI/           Reel presentation
  Tests/Editor/         Edit Mode tests
Build/WebGL/            Ready-to-host WebGL output
```

## Building WebGL

In Unity, choose **Jackpot Slots → Build WebGL**. For automation:

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath . `
  -executeMethod JackpotSlots.Editor.SlotBuildTools.BuildWebGL
```

## Math model and fairness

This is a demonstration game with virtual credits only. Each reel uses the same fixed 20-stop strip and is sampled independently. For symbol `i`, a three-of-a-kind has probability `(stops_i / 20)^3`. The complete model is:

| Symbol | Stops / reel | Three-match probability | Payout |
| --- | ---: | ---: | ---: |
| Seven | 2 | 0.100% | 1000 |
| Bar | 3 | 0.3375% | 400 |
| Bell | 5 | 1.5625% | 200 |
| Cherries | 10 | 12.500% | 33 |

The hit frequency is `14.5%`. Expected payout is `9.6` credits on a 10-credit bet:

`(0.1³ × 1000) + (0.15³ × 400) + (0.25³ × 200) + (0.5³ × 33) = 9.6`

Therefore theoretical RTP is `9.6 / 10 = 96%`, with a 4% house edge. RTP is a long-run expectation only; the game never adjusts future spins in response to wins or losses.

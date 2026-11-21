A slot machine built in Unity 6 for the Slot Game Assignment. Every reel outcome is generated using an outcome model that accounts for RNG, symbol probabilities, payouts, and RTP (theoretical), basically a real slot machine model just scaled down. For the sake of simplicity, a win occurs only when all three symbols match.

### Play

1. Open a terminal in the project directory.
2. Start a local web server:

   ```powershell
   py -m http.server 8080 --directory Build
3. Visit `http://localhost:8080` and **PULL THE LEVER**. Each spin costs 10 credits.


## Rules and payouts

|matching symbols | Reel weight | Payout |
| --- | ---: | ---: |
| Seven | 2 / 20 (10%) | 1000 credits |
| Bar | 3 / 20 (15%) | 400 credits |
| Bell | 5 / 20 (25%) | 200 credits |
| Cherries | 10 / 20 (50%) | 33 credits |

The player starts with 100 credits. **RESET CREDITS** restores the starting balance if you run out.

## Math model

For this assignment, I made it so each reel uses the same 20 slot strip and is sampled independently. For symbol `i`, a three-of-a-kind has probability `(stops_i / 20)^3`. The complete model is:

| Symbol | Stops / reel | Three-match probability | Payout |
| --- | ---: | ---: | ---: |
| Seven | 2 | 0.100% | 1000 |
| Bar | 3 | 0.3375% | 400 |
| Bell | 5 | 1.5625% | 200 |
| Cherries | 10 | 12.500% | 33 |

The hit frequency is `14.5%`; the expected payout is `9.6`, which makes credits on a 10-credit bet:

`(0.1³ × 1000) + (0.15³ × 400) + (0.25³ × 200) + (0.5³ × 33) = 9.6`

Therefore, theoretical RTP is `9.6 / 10 = 96%`, with a 4% house edge. RTP is a long-run expectation only; the game never adjusts future spins in response to wins or losses.


## Approach

- `SlotGameModel` contains wallet, win, and payout rules without any UI dependencies.
- `SlotRandom` owns a session-seeded PRNG. Each reel independently selects one of 20 virtual stops. The complete result is generated before animation starts.
- `ReelView` handles symbol cycling, staggered stops, and a landing bounce.
- `SlotGameController` is the composition root and builds a canvas from the provided artwork at runtime.


## Project structure

```text
Assets/
  Editor/
  SlotMachineArt/    Imported assignment artwork
  Scripts/Core/         Rules, outcomes, and RNG
  Scripts/UI/           Reel presentation
  Tests/Editor/         Edit Mode tests
  Animations/           Animations for the machine
  Prefabs/              Prefabs for the reels, Symbols
  Sound/                Sound effects
Build/            WebGL output
```

## To Do
1. Tune RTP further.
2. Add more fun features, maybe take it in a roguelike direction.
- Wild symbols
- Scatter symbols
- Bonus rounds
- Free spins
- Multiple paylines
- Progressive jackpots
- Variable bets
- Pay-both-ways rules

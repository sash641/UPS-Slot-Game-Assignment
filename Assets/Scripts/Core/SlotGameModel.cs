using System;
using System.Collections.Generic;

namespace JackpotSlots.Core
{
    /// <summary>Pure game rules and wallet state; deliberately independent from Unity UI.</summary>
    public sealed class SlotGameModel
    {
        public const int ReelCount = 3;
        public const int StartingCredits = 100;
        public const int Bet = 10;

        // A 20-stop virtual reel shared by all three independently sampled reels.
        // Frequencies: Seven 2/20, Cherries 10/20, Bell 5/20, Bar 3/20.
        private static readonly SlotSymbol[] VirtualReel =
        {
            SlotSymbol.Seven, SlotSymbol.Cherries, SlotSymbol.Bell, SlotSymbol.Cherries,
            SlotSymbol.Bar, SlotSymbol.Cherries, SlotSymbol.Bell, SlotSymbol.Cherries,
            SlotSymbol.Seven, SlotSymbol.Cherries, SlotSymbol.Bell, SlotSymbol.Cherries,
            SlotSymbol.Bar, SlotSymbol.Cherries, SlotSymbol.Bell, SlotSymbol.Cherries,
            SlotSymbol.Bar, SlotSymbol.Cherries, SlotSymbol.Bell, SlotSymbol.Cherries
        };

        private static readonly IReadOnlyDictionary<SlotSymbol, int> Payouts =
            new Dictionary<SlotSymbol, int>
            {
                { SlotSymbol.Seven, 1000 },
                { SlotSymbol.Cherries, 33 },
                { SlotSymbol.Bell, 200 },
                { SlotSymbol.Bar, 400 }
            };

        private readonly SlotRandom random;

        public SlotGameModel(SlotRandom random = null)
        {
            this.random = random ?? new SlotRandom();
            Credits = StartingCredits;
        }

        public int Credits { get; private set; }
        public bool CanSpin => Credits >= Bet;

        public SpinResult Spin()
        {
            if (!CanSpin)
                throw new InvalidOperationException("Not enough credits to spin.");

            Credits -= Bet;

            var symbols = new SlotSymbol[ReelCount];
            for (var i = 0; i < ReelCount; i++)
                symbols[i] = VirtualReel[random.Next(VirtualReel.Length)];

            var allMatch = symbols[0] == symbols[1] && symbols[1] == symbols[2];
            var payout = allMatch ? Payouts[symbols[0]] : 0;
            Credits += payout;
            return new SpinResult(symbols, payout);
        }

        public void ResetCredits()
        {
            Credits = StartingCredits;
        }

        public static int PayoutFor(SlotSymbol symbol) => Payouts[symbol];

        public static int StopsFor(SlotSymbol symbol)
        {
            var count = 0;
            for (var i = 0; i < VirtualReel.Length; i++)
                if (VirtualReel[i] == symbol)
                    count++;
            return count;
        }

        /// <summary>Exact long-run return implied by reel weights, payouts, and the fixed bet.</summary>
        public static double TheoreticalRtp
        {
            get
            {
                var expectedCredits = 0d;
                foreach (SlotSymbol symbol in Enum.GetValues(typeof(SlotSymbol)))
                {
                    var probability = (double)StopsFor(symbol) / VirtualReel.Length;
                    expectedCredits += probability * probability * probability * PayoutFor(symbol);
                }
                return expectedCredits / Bet;
            }
        }

        public static double HitFrequency
        {
            get
            {
                var frequency = 0d;
                foreach (SlotSymbol symbol in Enum.GetValues(typeof(SlotSymbol)))
                {
                    var probability = (double)StopsFor(symbol) / VirtualReel.Length;
                    frequency += probability * probability * probability;
                }
                return frequency;
            }
        }
    }
}

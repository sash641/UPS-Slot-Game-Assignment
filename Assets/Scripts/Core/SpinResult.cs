using System;

namespace JackpotSlots.Core
{
    /// <summary>Immutable value object describing one independently generated spin.</summary>
    public readonly struct SpinResult
    {
        public SpinResult(SlotSymbol[] symbols, int payout)
        {
            Symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
            Payout = payout;
        }

        public SlotSymbol[] Symbols { get; }
        public int Payout { get; }
        public bool IsWin => Payout > 0;
    }
}

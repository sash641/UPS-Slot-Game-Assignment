using JackpotSlots.Core;
using NUnit.Framework;

namespace JackpotSlots.Tests
{
    public sealed class SlotGameModelTests
    {
        [Test]
        public void NewGameStartsWithExpectedCredits()
        {
            var game = new SlotGameModel();
            Assert.That(game.Credits, Is.EqualTo(SlotGameModel.StartingCredits));
            Assert.That(game.CanSpin, Is.True);
        }

        [Test]
        public void PayoutTableRewardsHigherValueSymbols()
        {
            Assert.That(SlotGameModel.PayoutFor(SlotSymbol.Seven), Is.GreaterThan(SlotGameModel.PayoutFor(SlotSymbol.Cherries)));
            Assert.That(SlotGameModel.PayoutFor(SlotSymbol.Bar), Is.GreaterThan(SlotGameModel.PayoutFor(SlotSymbol.Bell)));
        }

        [Test]
        public void MathModelTargetsNinetySixPercentRtp()
        {
            Assert.That(SlotGameModel.TheoreticalRtp, Is.EqualTo(0.96d).Within(0.0000001d));
            Assert.That(SlotGameModel.HitFrequency, Is.EqualTo(0.145d).Within(0.0000001d));
        }
    }
}

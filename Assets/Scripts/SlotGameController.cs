using System.Collections;
using JackpotSlots.Core;
using UnityEngine;
using UnityEngine.UI;

namespace JackpotSlots
{
    public sealed class SlotGameController : MonoBehaviour
    {
        private static readonly int PullTrigger = Animator.StringToHash("Pull");

        [Header("Physical Reels")]
        [SerializeField] private ReelController[] reels =
            new ReelController[SlotGameModel.ReelCount];

        [Header("Final Symbol Mapping")]
        [SerializeField] private Sprite[] symbolSprites = new Sprite[4];

        [Header("Lever")]
        [SerializeField] private Animator leverAnimator;

        [Header("Audio")]
        [SerializeField] private SlotAudioController audioController;

        [Header("HUD")]
        [SerializeField] private Text creditsText;
        [SerializeField] private Text betText;
        [SerializeField] private Text statusText;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button resetButton;

        [Header("Win Popup")]
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private Text winText;

        private SlotGameModel gameModel;
        private bool spinning;

        private void Awake()
        {
            gameModel = new SlotGameModel();
            RefreshUI();
            SetWinPopupVisible(false);
            statusText.text = "PRESS SPIN OR\nPULL THE LEVER";
        }

        /// <summary>Called by the Spin button and the clickable lever.</summary>
        public void StartSpin()
        {
            if (spinning)
                return;

            if (audioController != null)
                audioController.PlayUiButtonClick();

            if (!gameModel.CanSpin)
            {
                statusText.text = "OUT OF CREDITS";
                return;
            }

            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            spinning = true;
            spinButton.interactable = false;
            resetButton.interactable = false;
            SetWinPopupVisible(false);

            // Determine the real outcome before any animation or audio starts.
            var result = gameModel.Spin();
            RefreshUI();
            statusText.text = "SPINNING...";

            if (leverAnimator != null)
                leverAnimator.SetTrigger(PullTrigger);

            if (audioController != null)
            {
                audioController.PlayLeverPull();
                audioController.StartReelLoop();
            }

            var stoppedReels = 0;

            for (var i = 0; i < reels.Length; i++)
            {
                var finalSprite = symbolSprites[(int)result.Symbols[i]];
                var duration = 1.4f + i * 0.4f;

                StartCoroutine(
                    reels[i].SpinTo(
                        finalSprite,
                        duration,
                        () => stoppedReels++));
            }

            while (stoppedReels < reels.Length)
                yield return null;

            if (audioController != null)
                audioController.StopReelLoop();

            if (result.IsWin)
            {
                statusText.text = $"WIN! +{result.Payout} CREDITS";
                winText.text =
                    $"THREE {GetPluralSymbolName(result.Symbols[0])}\n" +
                    $"YOU WON {result.Payout} CREDITS";

                if (audioController != null)
                    audioController.PlayCoinPayout();

                SetWinPopupVisible(true);
            }
            else
            {
                statusText.text = gameModel.CanSpin
                    ? "NO MATCH TRY AGAIN"
                    : "OUT OF CREDITS";
            }

            spinning = false;
            spinButton.interactable = gameModel.CanSpin;
            resetButton.interactable = true;
        }

        /// <summary>Assign this to the Reset button OnClick event.</summary>
        public void ResetCredits()
        {
            if (spinning)
                return;

            if (audioController != null)
                audioController.PlayUiButtonClick();

            gameModel.ResetCredits();
            RefreshUI();
            SetWinPopupVisible(false);
            statusText.text = "BANKROLL!!!";
            spinButton.interactable = true;
        }

        /// <summary>Assign this to the Collect button OnClick event.</summary>
        public void HideWinPopup()
        {
            if (audioController != null)
                audioController.PlayUiButtonClick();

            SetWinPopupVisible(false);
        }

        private void SetWinPopupVisible(bool visible)
        {
            if (winPanel == null)
                return;

            winPanel.alpha = visible ? 1f : 0f;
            winPanel.interactable = visible;
            winPanel.blocksRaycasts = visible;
        }

        private void RefreshUI()
        {
            creditsText.text = $"CREDITS: {gameModel.Credits}";

            if (betText != null)
                betText.text = $"BET: {SlotGameModel.Bet}";
        }

        private static string GetPluralSymbolName(SlotSymbol symbol)
        {
            return symbol switch
            {
                SlotSymbol.Seven => "SEVENS",
                SlotSymbol.Cherries => "CHERRIES",
                SlotSymbol.Bell => "BELLS",
                SlotSymbol.Bar => "BARS",
                _ => symbol.ToString().ToUpperInvariant()
            };
        }
    }
}

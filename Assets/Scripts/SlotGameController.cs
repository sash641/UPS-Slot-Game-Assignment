using System.Collections;
using JackpotSlots.Core;
using UnityEngine;
using UnityEngine.UI;

namespace JackpotSlots
{
    public sealed class SlotGameController : MonoBehaviour
    {
        private static readonly int PullTrigger =
            Animator.StringToHash("Pull");

        [Header("Physical Reels")]
        [SerializeField]
        private ReelController[] reels =
            new ReelController[SlotGameModel.ReelCount];

        [Header("Final Symbol Mapping")]
        [SerializeField]
        private Sprite[] symbolSprites = new Sprite[4];

        [Header("Lever")]
        [SerializeField] private Animator leverAnimator;

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
            HideWinPopup();

            statusText.text = "PRESS SPIN OR\nPULL THE LEVER";
        }

        /// <summary>
        /// Assign this to the Spin Button OnClick event.
        /// </summary>
        public void StartSpin()
        {
            if (spinning)
                return;

            if (!gameModel.CanSpin)
            {
                statusText.text =
                    "OUT OF CREDITS";

                return;
            }

            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            spinning = true;

            spinButton.interactable = false;
            resetButton.interactable = false;

            HideWinPopup();

            // Determine the result before animation starts.
            var result = gameModel.Spin();

            RefreshUI();
            statusText.text = "SPINNING...";

            if (leverAnimator != null)
                leverAnimator.SetTrigger(PullTrigger);

            var stoppedReels = 0;

            for (var i = 0; i < reels.Length; i++)
            {
                var finalSprite =
                    symbolSprites[(int)result.Symbols[i]];

                var duration = 1.4f + i * 0.4f;

                StartCoroutine(
                    reels[i].SpinTo(
                        finalSprite,
                        duration,
                        () => stoppedReels++));
            }

            while (stoppedReels < reels.Length)
                yield return null;

            if (result.IsWin)
            {
                statusText.text =
                    $"WIN! +{result.Payout} CREDITS";

                winText.text =
                    $"THREE " +
                    $"{result.Symbols[0].ToString().ToUpperInvariant()}\n"+"S"+
                    $"YOU WON {result.Payout} CREDITS";

                ShowWinPopup();
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

        /// <summary>
        /// Assign this to the Reset Button OnClick event.
        /// </summary>
        public void ResetCredits()
        {
            if (spinning)
                return;

            gameModel.ResetCredits();

            RefreshUI();
            HideWinPopup();

            statusText.text = "BANKROLL!!!";
            spinButton.interactable = true;
        }

        private void ShowWinPopup()
        {
            if (winPanel == null)
                return;

            winPanel.alpha = 1f;
            winPanel.interactable = true;
            winPanel.blocksRaycasts = true;
        }

        /// <summary>
        /// Assign this to the Collect Button OnClick event.
        /// </summary>
        public void HideWinPopup()
        {
            if (winPanel == null)
                return;

            winPanel.alpha = 0f;
            winPanel.interactable = false;
            winPanel.blocksRaycasts = false;
        }

        private void RefreshUI()
        {
            creditsText.text =
                $"CREDITS: {gameModel.Credits}";

            if (betText != null)
                betText.text = $"BET: {SlotGameModel.Bet}";
        }
    }
}
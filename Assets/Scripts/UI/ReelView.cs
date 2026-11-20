using System;
using System.Collections;
using JackpotSlots.Core;
using UnityEngine;
using UnityEngine.UI;

namespace JackpotSlots.UI
{
    /// <summary>Visual reel only. Outcomes are supplied by the model before animation begins.</summary>
    public sealed class ReelView : MonoBehaviour
    {
        [Header("Prefab references")]
        [SerializeField] private Image symbolImage;
        [SerializeField] private Sprite[] sprites = new Sprite[4];

        private RectTransform symbolTransform;

        private void Awake()
        {
            if (symbolImage != null)
                symbolTransform = symbolImage.rectTransform;
        }

        public void Configure(Image image, Sprite[] symbolSprites)
        {
            symbolImage = image;
            sprites = symbolSprites;
            symbolTransform = image.rectTransform;
        }

        public void Show(SlotSymbol symbol)
        {
            symbolImage.sprite = sprites[(int)symbol];
            symbolImage.color = Color.white;
            symbolTransform.anchoredPosition = Vector2.zero;
            symbolTransform.localScale = Vector3.one;
        }

        public IEnumerator SpinTo(SlotSymbol result, float duration, Action onStopped = null)
        {
            var elapsed = 0f;
            var nextSwap = 0f;
            var frame = UnityEngine.Random.Range(0, sprites.Length);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = Mathf.Clamp01(elapsed / duration);
                var easeOut = 1f - Mathf.Pow(1f - normalized, 3f);
                var swapInterval = Mathf.Lerp(0.045f, 0.18f, easeOut);

                if (elapsed >= nextSwap)
                {
                    frame = (frame + 1) % sprites.Length;
                    symbolImage.sprite = sprites[frame];
                    nextSwap = elapsed + swapInterval;
                }

                // Rapid vertical travel that slows naturally, followed by a small mechanical bounce.
                var travel = Mathf.Lerp(54f, 8f, easeOut);
                symbolTransform.anchoredPosition = new Vector2(0f, -Mathf.Repeat(elapsed * 820f, 108f) + 54f);
                symbolImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.58f, 1f, easeOut));
                symbolTransform.localScale = new Vector3(1f, 1f + travel / 220f, 1f);
                yield return null;
            }

            Show(result);
            yield return Bounce();
            onStopped?.Invoke();
        }

        private IEnumerator Bounce()
        {
            const float bounceDuration = 0.2f;
            var time = 0f;
            while (time < bounceDuration)
            {
                time += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(time / bounceDuration);
                var y = Mathf.Sin(t * Mathf.PI) * 12f * (1f - t);
                symbolTransform.anchoredPosition = new Vector2(0f, -y);
                symbolTransform.localScale = new Vector3(1f + 0.06f * Mathf.Sin(t * Mathf.PI), 1f - 0.04f * Mathf.Sin(t * Mathf.PI), 1f);
                yield return null;
            }

            symbolTransform.anchoredPosition = Vector2.zero;
            symbolTransform.localScale = Vector3.one;
        }
    }
}

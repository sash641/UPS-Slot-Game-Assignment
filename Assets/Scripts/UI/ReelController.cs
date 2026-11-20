using System;
using System.Collections;
using UnityEngine;

public sealed class ReelController : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private SymbolView[] symbols = new SymbolView[5];
    [SerializeField] private Sprite[] availableSprites = new Sprite[4];

    [Header("Animation")]
    [SerializeField] private float symbolSpacing = 1.2f;
    [SerializeField] private float maximumSpeed = 14f;

    private bool spinning;

    private void Awake()
    {
        AlignSymbols();

        // Assign starting sprites to the five display objects.
        for (var i = 0; i < symbols.Length; i++)
        {
            symbols[i].SetSprite(
                availableSprites[i % availableSprites.Length]);
        }
    }

    public IEnumerator SpinTo(
        Sprite finalSprite,
        float duration,
        Action onStopped = null)
    {
        if (spinning)
            yield break;

        spinning = true;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            var normalizedTime = Mathf.Clamp01(elapsed / duration);

            // Accelerates at the beginning and slows near the end.
            var speed = maximumSpeed *
                        Mathf.Sin(normalizedTime * Mathf.PI);

            MoveSymbols(speed * Time.deltaTime);
            yield return null;
        }

        AlignSymbols();

        // Element 2 is the center winning position.
        symbols[2].SetSprite(finalSprite);

        spinning = false;
        onStopped?.Invoke();
    }

    private void MoveSymbols(float distance)
    {
        foreach (var symbol in symbols)
        {
            symbol.transform.localPosition +=
                Vector3.down * distance;

            // Move a symbol back to the top after it leaves the bottom.
            if (symbol.transform.localPosition.y <
                -symbolSpacing * 2f)
            {
                symbol.transform.localPosition +=
                    Vector3.up *
                    symbolSpacing *
                    symbols.Length;

                // This random sprite is visual only.
                // It does not determine the real outcome.
                symbol.SetSprite(
                    availableSprites[
                        UnityEngine.Random.Range(
                            0,
                            availableSprites.Length)]);
            }
        }
    }

    private void AlignSymbols()
    {
        for (var i = 0; i < symbols.Length; i++)
        {
            symbols[i].transform.localPosition =
                Vector3.up * ((i - 2) * symbolSpacing);
        }
    }
}
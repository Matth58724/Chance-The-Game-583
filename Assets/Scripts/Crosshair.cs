using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 10f;   // Length of each line
    [SerializeField] private float crosshairGap = 4f;     // Gap in the center
    [SerializeField] private float lineWidth = 2f;        // Thickness of lines

    // ── PRIVATE REFS ─────────────────────────────────────────────
    private Image topLine;
    private Image bottomLine;
    private Image leftLine;
    private Image rightLine;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        // Build the crosshair from four UI Image lines
        topLine    = CreateLine("Top",    new Vector2(lineWidth, crosshairSize), new Vector2(0, crosshairGap + crosshairSize / 2));
        bottomLine = CreateLine("Bottom", new Vector2(lineWidth, crosshairSize), new Vector2(0, -(crosshairGap + crosshairSize / 2)));
        leftLine   = CreateLine("Left",   new Vector2(crosshairSize, lineWidth), new Vector2(-(crosshairGap + crosshairSize / 2), 0));
        rightLine  = CreateLine("Right",  new Vector2(crosshairSize, lineWidth), new Vector2(crosshairGap + crosshairSize / 2, 0));
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    // Creates a single line of the crosshair as a UI Image
    Image CreateLine(string lineName, Vector2 size, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject("Crosshair_" + lineName);
        obj.transform.SetParent(transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image img = obj.AddComponent<Image>();
        img.color = crosshairColor;

        return img;
    }
}

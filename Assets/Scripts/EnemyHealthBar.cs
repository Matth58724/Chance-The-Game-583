using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float barWidth = 1.5f;
    [SerializeField] private float barHeight = 0.15f;

    // ── HEALTH BAR COLORS ────────────────────────────────────────
    private Color highHealthColor = new Color(0.0f, 0.9f, 0.2f, 1f);
    private Color midHealthColor  = new Color(1.0f, 0.7f, 0.0f, 1f);
    private Color lowHealthColor  = new Color(0.9f, 0.1f, 0.1f, 1f);
    private Color bgColor         = new Color(0.15f, 0.15f, 0.15f, 1f);

    // ── PRIVATE REFS ─────────────────────────────────────────────
    private EnemyHealth enemyHealth;
    private Transform mainCameraTransform;

    // ── BAR QUADS ────────────────────────────────────────────────
    private GameObject bgQuad;
    private GameObject fillQuad;
    private Material bgMat;
    private Material fillMat;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        mainCameraTransform = Camera.main.transform;
        BuildHealthBar();
    }

void LateUpdate()
    {
        if (bgQuad == null) return;

        // Position above enemy and face camera
        Vector3 pos = transform.position + offset;
        Quaternion rot = Quaternion.LookRotation(mainCameraTransform.forward);

        // Background sits at base position
        bgQuad.transform.position = pos;
        bgQuad.transform.rotation = rot;

        // Update fill scale based on health
        if (enemyHealth != null)
        {
            float pct = Mathf.Clamp01((float)enemyHealth.currentHealth / enemyHealth.maxHealth);

            // Scale fill quad width based on health percentage
            fillQuad.transform.localScale = new Vector3(barWidth * pct, barHeight * 0.7f, 1f);
            fillQuad.transform.rotation = rot;

            // Anchor fill to left side of background
            // Offset slightly toward camera to prevent z-fighting
            Vector3 fillPos = pos
                + bgQuad.transform.right * (barWidth * pct - barWidth) * 0.5f
                - mainCameraTransform.forward * 0.01f;
            fillQuad.transform.position = fillPos;

            // Update color based on health percentage
            if (pct > 0.6f)
                fillMat.color = highHealthColor;
            else if (pct > 0.3f)
                fillMat.color = midHealthColor;
            else
                fillMat.color = lowHealthColor;
        }
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

    void BuildHealthBar()
    {
        // Background quad — dark panel behind fill
        bgQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bgQuad.name = "HPBarBackground";
        Destroy(bgQuad.GetComponent<MeshCollider>());
        bgQuad.transform.localScale = new Vector3(barWidth, barHeight, 1f);

        bgMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        bgMat.color = bgColor;
        bgQuad.GetComponent<Renderer>().material = bgMat;
        bgQuad.GetComponent<Renderer>().sortingOrder = 10;

        // Fill quad — colored bar that shrinks based on health
        fillQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fillQuad.name = "HPBarFill";
        Destroy(fillQuad.GetComponent<MeshCollider>());
        fillQuad.transform.localScale = new Vector3(barWidth, barHeight * 0.7f, 1f);

        fillMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        fillMat.color = highHealthColor;
        fillQuad.GetComponent<Renderer>().material = fillMat;
        fillQuad.GetComponent<Renderer>().sortingOrder = 11;
    }

    void OnDestroy()
    {
        // Clean up quads and materials when enemy dies
        if (bgQuad != null) Destroy(bgQuad);
        if (fillQuad != null) Destroy(fillQuad);
        if (bgMat != null) Destroy(bgMat);
        if (fillMat != null) Destroy(fillMat);
    }
}

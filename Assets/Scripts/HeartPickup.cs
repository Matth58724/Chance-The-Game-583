using UnityEngine;

// Builds a heart shape from primitives at runtime and saves as a prefab-ready GameObject
// Heart = 2 spheres (top bumps) + 1 rotated cube (bottom point) combined
public class HeartPickup : MonoBehaviour
{
    public int healAmount = 50;
    public float rotationSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        BuildHeartMesh();
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void BuildHeartMesh()
    {
        // Red half material
        var redMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        redMat.color = new Color(0.85f, 0.05f, 0.05f, 1f);
        redMat.SetColor("_EmissionColor", new Color(0.4f, 0f, 0f, 1f));
        redMat.EnableKeyword("_EMISSION");

        // White half material
        var whiteMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        whiteMat.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        whiteMat.SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f, 1f));
        whiteMat.EnableKeyword("_EMISSION");

        // ── Red dome (top half) ──────────────────────────────────
        var redDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        redDome.name = "RedDome";
        redDome.transform.SetParent(transform, false);
        redDome.transform.localPosition = new Vector3(0f, 0.18f, 0f);
        redDome.transform.localScale    = new Vector3(0.35f, 0.35f, 0.35f);
        redDome.GetComponent<Renderer>().material = redMat;
        Destroy(redDome.GetComponent<Collider>());

        // ── White dome (bottom half) ─────────────────────────────
        var whiteDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        whiteDome.name = "WhiteDome";
        whiteDome.transform.SetParent(transform, false);
        whiteDome.transform.localPosition = new Vector3(0f, -0.18f, 0f);
        whiteDome.transform.localScale    = new Vector3(0.35f, 0.35f, 0.35f);
        whiteDome.GetComponent<Renderer>().material = whiteMat;
        Destroy(whiteDome.GetComponent<Collider>());

        // ── Red cylinder body (top) ──────────────────────────────
        var redCyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        redCyl.name = "RedBody";
        redCyl.transform.SetParent(transform, false);
        redCyl.transform.localPosition = new Vector3(0f, 0.09f, 0f);
        redCyl.transform.localScale    = new Vector3(0.35f, 0.1f, 0.35f);
        redCyl.GetComponent<Renderer>().material = redMat;
        Destroy(redCyl.GetComponent<Collider>());

        // ── White cylinder body (bottom) ─────────────────────────
        var whiteCyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        whiteCyl.name = "WhiteBody";
        whiteCyl.transform.SetParent(transform, false);
        whiteCyl.transform.localPosition = new Vector3(0f, -0.09f, 0f);
        whiteCyl.transform.localScale    = new Vector3(0.35f, 0.1f, 0.35f);
        whiteCyl.GetComponent<Renderer>().material = whiteMat;
        Destroy(whiteCyl.GetComponent<Collider>());

        // ── Glow light ───────────────────────────────────────────
        var lightObj = new GameObject("PillGlow");
        lightObj.transform.SetParent(transform, false);
        var light = lightObj.AddComponent<Light>();
        light.type      = LightType.Point;
        light.color     = new Color(1f, 0.2f, 0.2f, 1f);
        light.intensity = 1.2f;
        light.range     = 3f;
    }
    void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;
        if (playerHealth.currentHealth >= playerHealth.maxHealth) return;

        playerHealth.Heal(healAmount);
        Destroy(gameObject);
    }
}

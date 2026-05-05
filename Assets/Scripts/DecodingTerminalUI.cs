using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DecodingTerminalUI : MonoBehaviour
{
    // ── SETTINGS ─────────────────────────────────────────────────
    [SerializeField] private float interactRange = 3f;   // How close player must be to interact

    // ── UI REFERENCES ────────────────────────────────────────────
    [SerializeField] private GameObject terminalPanel;   // The full terminal UI panel
    [SerializeField] private Transform engramListParent; // Parent object for engram buttons
    [SerializeField] private GameObject engramButtonPrefab; // Button prefab for each engram
    [SerializeField] private TextMeshProUGUI promptText;  // "Press E to decode" prompt
    [SerializeField] private TextMeshProUGUI resultText;  // Shows decode result message

    // ── PRIVATE REFS ─────────────────────────────────────────────
    private DecodingTerminal terminal;
    private WeaponManager playerWeaponManager;
    private Transform playerTransform;
    private bool isOpen = false;
    public static bool isTerminalOpen = false;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        terminal = GetComponent<DecodingTerminal>();

        // Find player references
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerWeaponManager = player.GetComponent<WeaponManager>();
        }

        // Hide UI on start
        if (terminalPanel != null) terminalPanel.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = dist <= interactRange;

        // Show prompt when player is nearby and terminal is closed
        if (promptText != null)
            promptText.gameObject.SetActive(inRange && !isOpen);

        // Press E to open/close terminal
        if (inRange && Input.GetKeyDown(KeyCode.E) && !isOpen)
            OpenTerminal();
        else if (isOpen && Input.GetKeyDown(KeyCode.E))
            CloseTerminal();
    }

    // ── PUBLIC METHODS ───────────────────────────────────────────

public void OpenTerminal()
    {
        isOpen = true;
        isTerminalOpen = true;
        terminalPanel.SetActive(true);

        // Unlock cursor so player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (resultText != null) resultText.gameObject.SetActive(false);

        RefreshEngramList();
    }

public void CloseTerminal()
    {
        isOpen = false;
        isTerminalOpen = false;
        terminalPanel.SetActive(false);

        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Called when player clicks an engram button
    public void OnDecodeClicked(int engramIndex)
    {
        if (playerWeaponManager.engramInventory.Count <= engramIndex) return;

        EngramData engram = playerWeaponManager.engramInventory[engramIndex];

        if (engram.possibleDrops == null || engram.possibleDrops.Count == 0)
        {
            ShowResult("No possible drops configured for this engram!");
            return;
        }

        // Decode the engram
        terminal.DecodeEngram(playerWeaponManager, engramIndex);

        // Show result message
        ShowResult("Decoded into: " + playerWeaponManager.inventory[playerWeaponManager.inventory.Count - 1].weaponName + "!");

        // Refresh the list
        RefreshEngramList();
    }

    // ── PRIVATE METHODS ──────────────────────────────────────────

void RefreshEngramList()
    {
        // Clear existing buttons
        foreach (Transform child in engramListParent)
            Destroy(child.gameObject);

        if (playerWeaponManager.engramInventory.Count == 0)
        {
            GameObject emptyObj = new GameObject("EmptyText");
            emptyObj.transform.SetParent(engramListParent, false);
            TextMeshProUGUI emptyText = emptyObj.AddComponent<TextMeshProUGUI>();
            emptyText.text = "No engrams to decode.";
            emptyText.fontSize = 18;
            emptyText.color = Color.gray;
            emptyText.alignment = TextAlignmentOptions.Center;
            return;
        }

        // Spawn a button for each engram in inventory
        for (int i = 0; i < playerWeaponManager.engramInventory.Count; i++)
        {
            EngramData engram = playerWeaponManager.engramInventory[i];
            if (engram == null) continue;

            GameObject btnObj = Instantiate(engramButtonPrefab, engramListParent);

            // Set button size smaller to reduce spacing
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            if (btnRect != null)
                btnRect.sizeDelta = new Vector2(btnRect.sizeDelta.x, 40f);

            // Set button label - just the engram name
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = engram.engramName;

            // Set button color to match engram rarity
            Image btnImage = btnObj.GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = engram.engramColor;

            // Wire up button click
            int index = i;
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnDecodeClicked(index));
        }
    }

    void ShowResult(string message)
    {
        if (resultText == null) return;
        resultText.gameObject.SetActive(true);
        resultText.text = message;
        Invoke(nameof(HideResult), 3f);
    }

    void HideResult()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }
}

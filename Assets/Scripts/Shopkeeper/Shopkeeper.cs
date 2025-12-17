using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnackAttack.Player;   // for WeaponManager

public class Shopkeeper : MonoBehaviour
{

    [Header("Debug")]
    public bool debugMode = true;
    public int debugAddPointsAmount = 5000;
    public KeyCode debugAddPointsKey = KeyCode.K; // press K to add points

    [Header("Player & Distance")]
    public Transform player;          // this is the player transform
    public float interactionRadius = 3f;

    [Header("UI References")]
    public GameObject promptUI;       // "Press P to interact..." text object
    public GameObject shopPanel;      // The whole shop UI panel
    public TMP_Text pointsText;
    public TMP_Text[] optionTexts;    // 0: Rifle, 1: Shotgun, 2?4 upgrades

    [Header("Keybinds")]
    public KeyCode interactKey = KeyCode.P;
    public KeyCode confirmKey = KeyCode.Return;

    [Header("Costs")]
    public int glizzyRifleCost = 2000;
    public int shotgunCost = 1000;
    public int pistolUpgradeCost = 500;
    public int shotgunUpgradeCost = 750;
    public int rifleUpgradeCost = 1000;

    [Header("References")]
    public PlayerPoints playerPoints;     // holds current points
    public WeaponStats weaponStats;       // holds damage values
    public WeaponManager weaponManager;   // unlocks/handles weapons
    public FPSPlayerControllerWithWeapons playerWeaponsController;


    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip interactSound;

    private bool isPlayerInRange = false;
    private bool isShopOpen = false;
    private int selectedIndex = 0;

    private bool rifleUnlockedTest = false;
    private bool shotgunUnlockedTest = false;


    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);

        if (playerWeaponsController == null && player != null)
            playerWeaponsController = player.GetComponent<FPSPlayerControllerWithWeapons>();

        // Ensure we have an AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f; // 3D sound

        UpdateOptionTexts();
    }

    private void Update()
    {
        CheckDistance();

        if (debugMode && playerPoints != null && Input.GetKeyDown(debugAddPointsKey))
        {
            playerPoints.points += debugAddPointsAmount;
            Debug.Log($"[Shopkeeper DEBUG] Added {debugAddPointsAmount} points. Total: {playerPoints.points}");
            UpdatePointsText();
        }


        if (!isShopOpen && isPlayerInRange)
        {
            if (promptUI != null) promptUI.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                OpenShop();
            }
        }
        else
        {
            if (promptUI != null) promptUI.SetActive(false);
        }

        if (isShopOpen)
        {
            HandleShopInput();
        }
    }

    private void CheckDistance()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        isPlayerInRange = dist <= interactionRadius;
    }

    private void OpenShop()
    {
        isShopOpen = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(true);

        // play interact sound
        if (audioSource != null && interactSound != null)
        {
            audioSource.PlayOneShot(interactSound);
        }

        selectedIndex = 0;
        HighlightSelection();
        UpdatePointsText();
    }

    private void CloseShop()
    {
        isShopOpen = false;

        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (promptUI != null && isPlayerInRange)
            promptUI.SetActive(true);
    }

    private void HandleShopInput()
    {

        if (optionTexts == null || optionTexts.Length == 0) return;

        // Navigate options with arrow keys
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= optionTexts.Length) selectedIndex = 0;
            HighlightSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = optionTexts.Length - 1;
            HighlightSelection();
        }

        // Confirm purchase
        if (Input.GetKeyDown(confirmKey))
        {
            TryPurchase(selectedIndex);
            UpdatePointsText();
            UpdateOptionTexts();
            HighlightSelection();
        }


        // Exit shop with Escape only (prevents instant open+close on the same P press)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }

    }

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private void HighlightSelection()
    {
        bool rifleOwned = playerWeaponsController != null && playerWeaponsController.rifleUnlocked;
        bool shotgunOwned = playerWeaponsController != null && playerWeaponsController.shotgunUnlocked;

        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (optionTexts[i] == null) continue;

            bool disabled =
                (i == 0 && rifleOwned) ||
                (i == 1 && shotgunOwned);

            optionTexts[i].color = disabled ? Color.gray : (i == selectedIndex ? selectedColor : normalColor);
            optionTexts[i].fontStyle = FontStyles.Normal;
        }
    }

    private void UpdatePointsText()
    {
        if (pointsText != null && playerPoints != null)
        {
            pointsText.text = "Points: " + playerPoints.points;
        }
    }

    private void UpdateOptionTexts()
    {
        bool rifleOwned = playerWeaponsController != null && playerWeaponsController.rifleUnlocked;
        bool shotgunOwned = playerWeaponsController != null && playerWeaponsController.shotgunUnlocked;

        if (optionTexts.Length > 0 && optionTexts[0] != null)
            optionTexts[0].text = rifleOwned
                ? "Glizzy Rifle - OWNED"
                : $"Buy Glizzy Rifle - {glizzyRifleCost} pts";

        if (optionTexts.Length > 1 && optionTexts[1] != null)
            optionTexts[1].text = shotgunOwned
                ? "Shotgun - OWNED"
                : $"Buy Shotgun - {shotgunCost} pts";

        if (optionTexts.Length > 2 && optionTexts[2] != null)
            optionTexts[2].text = $"Upgrade Pistol Damage - {pistolUpgradeCost} pts";

        if (optionTexts.Length > 3 && optionTexts[3] != null)
            optionTexts[3].text = $"Upgrade Shotgun Damage - {shotgunUpgradeCost} pts";

        if (optionTexts.Length > 4 && optionTexts[4] != null)
            optionTexts[4].text = $"Upgrade Rifle Damage - {rifleUpgradeCost} pts";
    }


    private void TryPurchase(int index)
    {
        if (playerPoints == null)
        {
            Debug.LogError("[Shopkeeper] PlayerPoints not assigned.");
            return;
        }

        bool Spend(int cost)
        {
            if (playerPoints.points < cost)
            {
                Debug.Log("Not enough points.");
                return false;
            }
            playerPoints.points -= cost;
            return true;
        }

        switch (index)
        {
            case 0: // Glizzy Rifle
                {
                    if (playerWeaponsController != null && playerWeaponsController.rifleUnlocked)
                    {
                        Debug.Log("Rifle already owned.");
                        return;
                    }
                    if (!Spend(glizzyRifleCost)) return;

                    if (playerWeaponsController != null)
                        playerWeaponsController.UnlockRifle();

                    Debug.Log("Purchased Glizzy Rifle!");
                    break;
                }

            case 1: // Shotgun
                {
                    if (playerWeaponsController != null && playerWeaponsController.shotgunUnlocked)
                    {
                        Debug.Log("Shotgun already owned.");
                        return;
                    }
                    if (!Spend(shotgunCost)) return;

                    if (playerWeaponsController != null)
                        playerWeaponsController.UnlockShotgun();

                    Debug.Log("Purchased Shotgun!");
                    break;
                }

            case 2: // Pistol upgrade
                if (weaponStats == null) { Debug.LogWarning("[Shopkeeper] WeaponStats not assigned."); return; }
                if (!Spend(pistolUpgradeCost)) return;
                weaponStats.UpgradePistol();
                Debug.Log("Upgraded Pistol Damage to: " + weaponStats.pistolDamage);
                break;

            case 3: // Shotgun upgrade
                if (weaponStats == null) { Debug.LogWarning("[Shopkeeper] WeaponStats not assigned."); return; }
                if (!Spend(shotgunUpgradeCost)) return;
                weaponStats.UpgradeShotgun();
                Debug.Log("Upgraded Shotgun Damage to: " + weaponStats.shotgunDamage);
                break;

            case 4: // Rifle upgrade
                if (weaponStats == null) { Debug.LogWarning("[Shopkeeper] WeaponStats not assigned."); return; }
                if (!Spend(rifleUpgradeCost)) return;
                weaponStats.UpgradeRifle();
                Debug.Log("Upgraded Rifle Damage to: " + weaponStats.rifleDamage);
                break;
        }
    }

}

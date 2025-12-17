using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SnackAttack.Player;   // for WeaponManager

public class Shopkeeper : MonoBehaviour
{
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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip interactSound;

    private bool isPlayerInRange = false;
    private bool isShopOpen = false;
    private int selectedIndex = 0;

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);

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
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (optionTexts[i] == null) continue;

            optionTexts[i].color = (i == selectedIndex) ? selectedColor : normalColor;
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
        if (optionTexts.Length > 0 && optionTexts[0] != null)
            optionTexts[0].text = $"Buy Glizzy Rifle - {glizzyRifleCost} pts";

        if (optionTexts.Length > 1 && optionTexts[1] != null)
            optionTexts[1].text = $"Buy Shotgun - {shotgunCost} pts";

        if (optionTexts.Length > 2 && optionTexts[2] != null)
            optionTexts[2].text = $"Upgrade Pistol Damage - {pistolUpgradeCost} pts";

        if (optionTexts.Length > 3 && optionTexts[3] != null)
            optionTexts[3].text = $"Upgrade Shotgun Damage - {shotgunUpgradeCost} pts";

        if (optionTexts.Length > 4 && optionTexts[4] != null)
            optionTexts[4].text = $"Upgrade Rifle Damage - {rifleUpgradeCost} pts";
    }

    private void TryPurchase(int index)
    {
        if (playerPoints == null || weaponStats == null) return;

        switch (index)
        {
            case 0: // Glizzy Rifle
                if (playerPoints.points >= glizzyRifleCost)
                {
                    playerPoints.points -= glizzyRifleCost;

                    if (weaponManager != null)
                        weaponManager.UnlockRifle();

                    Debug.Log("Purchased Glizzy Rifle!");
                }
                else
                {
                    Debug.Log("Not enough points for Glizzy Rifle.");
                }
                break;

            case 1: // Shotgun
                if (playerPoints.points >= shotgunCost)
                {
                    playerPoints.points -= shotgunCost;

                    if (weaponManager != null)
                        weaponManager.UnlockShotgun();

                    Debug.Log("Purchased Shotgun!");
                }
                else
                {
                    Debug.Log("Not enough points for Shotgun.");
                }
                break;

            case 2: // Pistol damage upgrade
                if (playerPoints.points >= pistolUpgradeCost)
                {
                    playerPoints.points -= pistolUpgradeCost;
                    weaponStats.UpgradePistol();
                    Debug.Log("Upgraded Pistol Damage to: " + weaponStats.pistolDamage);
                }
                else
                {
                    Debug.Log("Not enough points for Pistol upgrade.");
                }
                break;

            case 3: // Shotgun damage upgrade
                if (playerPoints.points >= shotgunUpgradeCost)
                {
                    playerPoints.points -= shotgunUpgradeCost;
                    weaponStats.UpgradeShotgun();
                    Debug.Log("Upgraded Shotgun Damage to: " + weaponStats.shotgunDamage);
                }
                else
                {
                    Debug.Log("Not enough points for Shotgun upgrade.");
                }
                break;

            case 4: // Rifle damage upgrade
                if (playerPoints.points >= rifleUpgradeCost)
                {
                    playerPoints.points -= rifleUpgradeCost;
                    weaponStats.UpgradeRifle();
                    Debug.Log("Upgraded Rifle Damage to: " + weaponStats.rifleDamage);
                }
                else
                {
                    Debug.Log("Not enough points for Rifle upgrade.");
                }
                break;
        }
    }
}

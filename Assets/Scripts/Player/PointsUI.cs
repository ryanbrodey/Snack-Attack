using UnityEngine;
using TMPro;

public class PointsUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("If left empty, will automatically find PlayerPoints in the scene")]
    public PlayerPoints playerPoints;   // your points script on the Player
    public TMP_Text pointsText;        // the "Points: X" text

    private void Start()
    {
        // Auto-find PlayerPoints if not assigned
        if (playerPoints == null)
        {
            FindPlayerPoints();
        }

        if (pointsText == null)
        {
            pointsText = GetComponent<TMP_Text>();
        }
    }

    private void FindPlayerPoints()
    {
        // Try to find PlayerPoints on the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPoints = player.GetComponent<PlayerPoints>();
        }
        
        // Fallback: search entire scene
        if (playerPoints == null)
        {
            playerPoints = FindObjectOfType<PlayerPoints>();
        }

    }

    private void Update()
    {
        // Try to find player points again if it's still null (in case player spawns later)
        if (playerPoints == null)
        {
            FindPlayerPoints();
        }

        if (playerPoints == null || pointsText == null) return;

        pointsText.text = "Points: " + playerPoints.points;
    }
}

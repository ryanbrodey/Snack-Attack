using UnityEngine;
using TMPro;

public class PointsUI : MonoBehaviour
{
    [Header("References")]
    public PlayerPoints playerPoints;   // your points script on the Player
    public TMP_Text pointsText;        // the "Points: X" text

    private void Update()
    {
        if (playerPoints == null || pointsText == null) return;

        pointsText.text = "Points: " + playerPoints.points;
    }
}

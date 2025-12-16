using UnityEngine;
using SnackAttack.Player;

namespace SnackAttack.UI
{
    /// <summary>
    /// Helper script to easily add crosshair to existing FPS players
    /// Use this in the Unity Editor to quickly setup crosshairs
    /// </summary>
    public class CrosshairSetup : MonoBehaviour
    {
        [Header("Setup Instructions")]
        [TextArea(10, 15)]
        [SerializeField] private string instructions = @"
=== CROSSHAIR SETUP GUIDE ===

AUTOMATIC SETUP:
1. Attach this script to your FPS Player GameObject
2. Click 'Add Crosshair To This Player' button below
3. The crosshair will be automatically configured!

MANUAL SETUP:
1. Add 'CrosshairManager' component to your FPS Player
2. The crosshair will appear automatically when you play

CONTROLS:
- C key: Toggle crosshair visibility (for testing)
- Crosshair follows your camera/mouse movement automatically

The crosshair is purple and static as requested!";

        [Header("Crosshair Settings")]
        public Color crosshairColor = new Color(0.6f, 0.2f, 0.8f, 1f); // Purple
        public float crosshairSize = 20f;
        public float crosshairThickness = 2f;
        
        [ContextMenu("Add Crosshair To This Player")]
        public void AddCrosshairToPlayer()
        {
            // Check if we already have a CrosshairManager
            CrosshairManager existing = GetComponent<CrosshairManager>();
            if (existing != null)
            {
                Debug.Log("[CrosshairSetup] CrosshairManager already exists on this player!");
                return;
            }
            
            // Add CrosshairManager
            CrosshairManager crosshairManager = gameObject.AddComponent<CrosshairManager>();
            
            // Apply settings
            crosshairManager.crosshairColor = crosshairColor;
            crosshairManager.crosshairSize = crosshairSize;
            crosshairManager.crosshairThickness = crosshairThickness;
            crosshairManager.enableCrosshair = true;
            
            Debug.Log("[CrosshairSetup] ✅ Crosshair added successfully!");
            Debug.Log("[CrosshairSetup] The crosshair will appear when you play the scene.");
            Debug.Log("[CrosshairSetup] Press 'C' during play to toggle crosshair visibility.");
            
            // Remove this setup script since we're done
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }
        
        [ContextMenu("Remove Crosshair From This Player")]
        public void RemoveCrosshairFromPlayer()
        {
            CrosshairManager crosshairManager = GetComponent<CrosshairManager>();
            if (crosshairManager != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(crosshairManager);
                }
                else
                {
                    DestroyImmediate(crosshairManager);
                }
                Debug.Log("[CrosshairSetup] Crosshair removed from player.");
            }
            else
            {
                Debug.Log("[CrosshairSetup] No crosshair found on this player.");
            }
        }
    }
}

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

        [Header("Crosshair Settings")]
        public Color crosshairColor = new Color(0.6f, 0.2f, 0.8f, 1f); // Purple
        public float crosshairSize = 20f;
        public float crosshairThickness = 2f;
        
        [ContextMenu("Add Crosshair To This Player")]
        public void AddCrosshairToPlayer()
        {
            CrosshairManager existing = GetComponent<CrosshairManager>();
            if (existing != null)
            {
                return;
            }
            
            // Add CrosshairManager
            CrosshairManager crosshairManager = gameObject.AddComponent<CrosshairManager>();
            
            // Apply settings
            crosshairManager.crosshairColor = crosshairColor;
            crosshairManager.crosshairSize = crosshairSize;
            crosshairManager.crosshairThickness = crosshairThickness;
            crosshairManager.enableCrosshair = true;
            
            // Remove this setup script
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
            }
        }
    }
}

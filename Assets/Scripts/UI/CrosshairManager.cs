using UnityEngine;
using SnackAttack.UI;

namespace SnackAttack.Player
{
    /// <summary>
    /// Manages crosshair integration with FPS player
    /// Automatically sets up dynamic crosshair when attached to FPS player
    /// </summary>
    public class CrosshairManager : MonoBehaviour
    {
        [Header("Crosshair Settings")]
        public bool enableCrosshair = true;
        public Color crosshairColor = new Color(0.6f, 0.2f, 0.8f, 1f); // Purple
        public float crosshairSize = 15f;
        public float crosshairThickness = 2f;
        
        [Header("Dynamic Behavior")]
        public bool useDynamicCrosshair = true;
        
        // References
        private CrosshairUI crosshairUI;
        private DynamicCrosshair dynamicCrosshair;
        private FPSCrosshair fpsCrosshair;
        private Camera playerCamera;
        
        void Start()
        {
            SetupCrosshair();
        }
        
        void SetupCrosshair()
        {
            if (!enableCrosshair) return;
            
            // Find player camera
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                // Try to find FPSController and get its camera
                var fpsController = GetComponent<FPSController>();
                if (fpsController != null)
                {
                    playerCamera = fpsController.PlayerCamera;
                }
            }
            
            // Create crosshair UI
            GameObject crosshairGO = new GameObject("CrosshairManager");
            crosshairGO.transform.SetParent(transform);
            
            // Always use FPS Crosshair for best experience
            fpsCrosshair = crosshairGO.AddComponent<FPSCrosshair>();
            fpsCrosshair.crosshairColor = crosshairColor;
            fpsCrosshair.crosshairSize = crosshairSize;
            fpsCrosshair.crosshairThickness = crosshairThickness;
            fpsCrosshair.showCrosshair = enableCrosshair;
            fpsCrosshair.playerCamera = playerCamera;
            fpsCrosshair.enableDynamicMovement = useDynamicCrosshair;
        }
        
        /// <summary>
        /// Enable or disable the crosshair
        /// </summary>
        public void SetCrosshairEnabled(bool enabled)
        {
            enableCrosshair = enabled;
            if (fpsCrosshair != null)
            {
                fpsCrosshair.SetCrosshairVisible(enabled);
            }
        }
        
        /// <summary>
        /// Change crosshair color
        /// </summary>
        public void SetCrosshairColor(Color newColor)
        {
            crosshairColor = newColor;
            if (fpsCrosshair != null)
            {
                fpsCrosshair.SetCrosshairColor(newColor);
            }
        }
        
        /// <summary>
        /// Change crosshair size
        /// </summary>
        public void SetCrosshairSize(float newSize)
        {
            crosshairSize = newSize;
            if (fpsCrosshair != null)
            {
                fpsCrosshair.SetCrosshairSize(newSize);
            }
        }
        
        void OnValidate()
        {
            // Update crosshair in real-time when values change in inspector
            if (Application.isPlaying && fpsCrosshair != null)
            {
                fpsCrosshair.crosshairColor = crosshairColor;
                fpsCrosshair.crosshairSize = crosshairSize;
                fpsCrosshair.crosshairThickness = crosshairThickness;
                fpsCrosshair.SetCrosshairColor(crosshairColor);
                fpsCrosshair.SetCrosshairSize(crosshairSize);
            }
        }
    }
}

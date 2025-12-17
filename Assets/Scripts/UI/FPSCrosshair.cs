using UnityEngine;
using UnityEngine.UI;

namespace SnackAttack.UI
{
    /// <summary>
    /// FPS-style crosshair that stays centered but shows where camera is aiming
    /// Properly handles cursor locking for FPS gameplay
    /// </summary>
    public class FPSCrosshair : MonoBehaviour
    {
        [Header("Crosshair Settings")]
        public Color crosshairColor = new Color(0.6f, 0.2f, 0.8f, 1f); // Purple color
        public float crosshairSize = 15f;
        public float crosshairThickness = 2f;
        public bool showCrosshair = true;
        
        [Header("Dynamic Behavior")]
        public bool enableDynamicMovement = true;
        public float movementSensitivity = 2f;
        public float maxOffset = 30f; // Max pixels from center
        public float returnSpeed = 8f; // How fast crosshair returns to center
        
        [Header("References")]
        public Camera playerCamera;
        
        // UI Elements
        private Canvas crosshairCanvas;
        private GameObject crosshairContainer;
        private Image horizontalLine;
        private Image verticalLine;
        private RectTransform containerRect;
        
        // Dynamic movement tracking
        private Vector2 crosshairOffset = Vector2.zero;
        private Vector2 targetOffset = Vector2.zero;
        
        void Start()
        {
            // Ensure cursor is properly locked for FPS
            LockCursor();
            
            // Find camera if not assigned
            FindPlayerCamera();
            
            // Create the crosshair UI
            CreateCrosshairUI();
            
            Debug.Log("[FPSCrosshair] FPS Crosshair initialized!");
        }
        
        void Update()
        {
            // Ensure cursor stays locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
            
            // Update crosshair position based on camera movement
            if (enableDynamicMovement && containerRect != null)
            {
                UpdateDynamicCrosshair();
            }
            
            // Toggle crosshair with 'C' key for testing
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetCrosshairVisible(!showCrosshair);
                Debug.Log($"[FPSCrosshair] Crosshair visibility: {showCrosshair}");
            }
            
            // Escape key to unlock cursor (for testing)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Debug.Log("[FPSCrosshair] Cursor unlocked");
                }
                else
                {
                    LockCursor();
                    Debug.Log("[FPSCrosshair] Cursor locked");
                }
            }
        }
        
        void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; // Hide the system cursor
            Debug.Log("[FPSCrosshair] Cursor locked and hidden");
        }
        
        void FindPlayerCamera()
        {
            if (playerCamera == null)
            {
                // Try to find camera in parent or children
                playerCamera = GetComponentInParent<Camera>();
                if (playerCamera == null)
                    playerCamera = GetComponentInChildren<Camera>();
                if (playerCamera == null)
                    playerCamera = FindObjectOfType<Camera>();
                if (playerCamera == null)
                    playerCamera = Camera.main;
                
                if (playerCamera != null)
                {
                    Debug.Log($"[FPSCrosshair] Found camera: {playerCamera.name}");
                }
                else
                {
                    Debug.LogWarning("[FPSCrosshair] No camera found!");
                }
            }
        }
        
        void UpdateDynamicCrosshair()
        {
            // Get mouse input (same as FPS controller uses)
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            // Create slight movement based on mouse input
            if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
            {
                // Add mouse movement to target offset
                Vector2 mouseMovement = new Vector2(mouseX, -mouseY) * movementSensitivity;
                targetOffset += mouseMovement;
                
                // Clamp the offset to prevent crosshair from going too far
                targetOffset = Vector2.ClampMagnitude(targetOffset, maxOffset);
            }
            
            // Gradually return crosshair toward center when not moving mouse
            if (Mathf.Abs(mouseX) < 0.01f && Mathf.Abs(mouseY) < 0.01f)
            {
                targetOffset = Vector2.Lerp(targetOffset, Vector2.zero, returnSpeed * Time.deltaTime);
            }
            
            // Smoothly move crosshair to target position
            crosshairOffset = Vector2.Lerp(crosshairOffset, targetOffset, 15f * Time.deltaTime);
            
            // Apply the offset
            containerRect.anchoredPosition = crosshairOffset;
        }
        
        void CreateCrosshairUI()
        {
            // Create Canvas
            CreateCanvas();
            
            // Create crosshair container
            crosshairContainer = new GameObject("FPSCrosshair");
            crosshairContainer.transform.SetParent(crosshairCanvas.transform, false);
            
            // Set up container positioning (center of screen)
            containerRect = crosshairContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(crosshairSize * 2, crosshairSize * 2);
            
            // Create horizontal line
            CreateCrosshairLine("HorizontalLine", new Vector2(crosshairSize, crosshairThickness), Vector2.zero, out horizontalLine);
            
            // Create vertical line
            CreateCrosshairLine("VerticalLine", new Vector2(crosshairThickness, crosshairSize), Vector2.zero, out verticalLine);
            
            // Set initial visibility
            SetCrosshairVisible(showCrosshair);
            
            Debug.Log("[FPSCrosshair] Crosshair UI created successfully!");
        }
        
        void CreateCanvas()
        {
            // Create Canvas GameObject
            GameObject canvasGO = new GameObject("FPSCrosshairCanvas");
            crosshairCanvas = canvasGO.AddComponent<Canvas>();
            crosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            crosshairCanvas.sortingOrder = 1000; // Make sure it's on top of everything
            
            // Add CanvasScaler for proper scaling
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // Add GraphicRaycaster (required for UI)
            canvasGO.AddComponent<GraphicRaycaster>();
            
            Debug.Log("[FPSCrosshair] Canvas created");
        }
        
        void CreateCrosshairLine(string name, Vector2 size, Vector2 position, out Image lineImage)
        {
            // Create line GameObject
            GameObject lineGO = new GameObject(name);
            lineGO.transform.SetParent(crosshairContainer.transform, false);
            
            // Set up RectTransform
            RectTransform lineRect = lineGO.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0.5f, 0.5f);
            lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.anchoredPosition = position;
            lineRect.sizeDelta = size;
            
            // Add Image component
            lineImage = lineGO.AddComponent<Image>();
            lineImage.color = crosshairColor;
            
            // Use Unity's default white sprite
            lineImage.sprite = null;
        }
        
        /// <summary>
        /// Show or hide the crosshair
        /// </summary>
        public void SetCrosshairVisible(bool visible)
        {
            showCrosshair = visible;
            if (crosshairContainer != null)
            {
                crosshairContainer.SetActive(visible);
            }
        }
        
        /// <summary>
        /// Change crosshair color
        /// </summary>
        public void SetCrosshairColor(Color newColor)
        {
            crosshairColor = newColor;
            if (horizontalLine != null) horizontalLine.color = newColor;
            if (verticalLine != null) verticalLine.color = newColor;
        }
        
        /// <summary>
        /// Change crosshair size
        /// </summary>
        public void SetCrosshairSize(float newSize)
        {
            crosshairSize = newSize;
            if (horizontalLine != null)
            {
                RectTransform hRect = horizontalLine.GetComponent<RectTransform>();
                hRect.sizeDelta = new Vector2(newSize, crosshairThickness);
            }
            if (verticalLine != null)
            {
                RectTransform vRect = verticalLine.GetComponent<RectTransform>();
                vRect.sizeDelta = new Vector2(crosshairThickness, newSize);
            }
            
            // Update container size
            if (containerRect != null)
            {
                containerRect.sizeDelta = new Vector2(newSize * 2, newSize * 2);
            }
        }
    }
}






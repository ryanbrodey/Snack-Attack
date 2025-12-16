using UnityEngine;
using UnityEngine.UI;

namespace SnackAttack.UI
{
    /// <summary>
    /// Advanced crosshair that shows exactly where the camera is pointing
    /// Uses raycasting to determine precise aim point
    /// </summary>
    public class DynamicCrosshair : MonoBehaviour
    {
        [Header("Crosshair Settings")]
        public Color crosshairColor = new Color(0.6f, 0.2f, 0.8f, 1f); // Purple color
        public float crosshairSize = 15f;
        public float crosshairThickness = 2f;
        public bool showCrosshair = true;
        
        [Header("Dynamic Behavior")]
        public bool useRaycastAiming = true;
        public float raycastDistance = 100f;
        public LayerMask aimLayers = -1; // What layers to aim at
        
        [Header("References")]
        public Canvas uiCanvas;
        public Camera playerCamera;
        
        // UI Elements
        private GameObject crosshairContainer;
        private Image horizontalLine;
        private Image verticalLine;
        private RectTransform containerRect;
        
        void Start()
        {
            // Find player camera if not assigned
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
                if (playerCamera == null)
                    playerCamera = Camera.main;
            }
            
            CreateCrosshairUI();
        }
        
        void Update()
        {
            if (useRaycastAiming && playerCamera != null && containerRect != null)
            {
                UpdateCrosshairWithRaycast();
            }
            
            // Toggle crosshair with 'C' key for testing
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetCrosshairVisible(!showCrosshair);
                Debug.Log($"[DynamicCrosshair] Crosshair visibility: {showCrosshair}");
            }
        }
        
        void UpdateCrosshairWithRaycast()
        {
            // Cast a ray from camera center forward
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            
            Vector3 targetPoint;
            
            // Try to hit something
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, aimLayers))
            {
                targetPoint = hit.point;
            }
            else
            {
                // If nothing hit, use a point far away in the direction we're looking
                targetPoint = ray.origin + ray.direction * raycastDistance;
            }
            
            // Convert world position to screen position
            Vector3 screenPoint = playerCamera.WorldToScreenPoint(targetPoint);
            
            // Convert screen position to UI position
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 offset = new Vector2(screenPoint.x, screenPoint.y) - screenCenter;
            
            // Apply the offset (this makes crosshair follow where camera is actually pointing)
            containerRect.anchoredPosition = offset * 0.1f; // Scale down the movement for better feel
        }
        
        void CreateCrosshairUI()
        {
            // Create Canvas if it doesn't exist
            if (uiCanvas == null)
            {
                CreateCanvas();
            }
            
            // Create crosshair container
            crosshairContainer = new GameObject("DynamicCrosshair");
            crosshairContainer.transform.SetParent(uiCanvas.transform, false);
            
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
            
            Debug.Log("[DynamicCrosshair] Dynamic crosshair created successfully!");
        }
        
        void CreateCanvas()
        {
            // Create Canvas GameObject
            GameObject canvasGO = new GameObject("DynamicCrosshairCanvas");
            uiCanvas = canvasGO.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCanvas.sortingOrder = 100; // Make sure it's on top
            
            // Add CanvasScaler for proper scaling
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // Add GraphicRaycaster (required for UI)
            canvasGO.AddComponent<GraphicRaycaster>();
            
            Debug.Log("[DynamicCrosshair] Canvas created");
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
            
            // Use a simple white texture (Unity's default UI sprite)
            lineImage.sprite = null; // This will use the default white square
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
        }
    }
}




using UnityEngine;
using UnityEngine.UI;

namespace SnackAttack.UI
{
    /// <summary>
    /// Dynamic crosshair UI component that follows mouse/camera movement
    /// Creates a purple cross that tracks where the player is aiming
    /// </summary>
    public class CrosshairUI : MonoBehaviour
    {
        [Header("Crosshair Settings")]
        public Color crosshairColor = new Color(0.6f, 0.2f, 0.8f, 1f); // Purple color
        public float crosshairSize = 20f;
        public float crosshairThickness = 2f;
        public bool showCrosshair = true;
        
        [Header("Dynamic Behavior")]
        public bool followCursor = true;
        public float followSensitivity = 1f;
        public float maxOffsetDistance = 50f; // Max pixels from center
        public float returnSpeed = 5f; // How fast crosshair returns to center
        
        [Header("References")]
        public Canvas uiCanvas;
        public Camera playerCamera;
        
        // UI Elements
        private GameObject crosshairContainer;
        private Image horizontalLine;
        private Image verticalLine;
        private RectTransform containerRect;
        
        // Dynamic movement
        private Vector2 currentOffset = Vector2.zero;
        private Vector2 targetOffset = Vector2.zero;
        private Vector2 lastMousePosition;
        private bool mouseInitialized = false;
        
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
            
            // Initialize mouse tracking
            if (followCursor)
            {
                lastMousePosition = Input.mousePosition;
                mouseInitialized = true;
            }
        }
        
        void Update()
        {
            if (followCursor && containerRect != null)
            {
                UpdateCrosshairPosition();
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetCrosshairVisible(!showCrosshair);
            }
        }
        
        void CreateCrosshairUI()
        {
            // Create Canvas if it doesn't exist
            if (uiCanvas == null)
            {
                CreateCanvas();
            }
            
            // Create crosshair container
            crosshairContainer = new GameObject("Crosshair");
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
        }
        
        void CreateCanvas()
        {
            // Create Canvas GameObject
            GameObject canvasGO = new GameObject("CrosshairCanvas");
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
        
        void UpdateCrosshairPosition()
        {
            if (playerCamera == null) return;
            
            // Get mouse input (same as what FPS controller uses)
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            // Only update if there's mouse movement
            if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
            {
                // Create slight offset based on mouse movement direction
                Vector2 mouseMovement = new Vector2(mouseX, mouseY);
                
                // Scale the movement and clamp it
                targetOffset += mouseMovement * followSensitivity;
                targetOffset = Vector2.ClampMagnitude(targetOffset, maxOffsetDistance);
            }
            
            // Gradually return crosshair to center when not moving mouse
            if (Mathf.Abs(mouseX) < 0.01f && Mathf.Abs(mouseY) < 0.01f)
            {
                targetOffset = Vector2.Lerp(targetOffset, Vector2.zero, returnSpeed * Time.deltaTime);
            }
            
            // Smoothly move crosshair towards target position
            currentOffset = Vector2.Lerp(currentOffset, targetOffset, 10f * Time.deltaTime);
            
            // Apply the offset to the crosshair container
            containerRect.anchoredPosition = currentOffset;
        }
    }
}

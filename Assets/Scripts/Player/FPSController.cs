using UnityEngine;

namespace SnackAttack.Player
{
    // basic fps controller - handles walking around and looking
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Stuff")]
        public float walkSpeed = 5f;
        public float runSpeed = 8f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f; // gravity is negative duh
        public float groundCheckDist = 0.4f;
        
        [Header("Mouse Look")]
        public float mouseSens = 2f; // made it fast like you wanted
        public float maxLookAngle = 80f;
        
        [Header("Refs")]
        public Camera cam;
        public Transform groundChecker;
        public LayerMask groundMask = 1;
        
        [Header("UI")]
        public bool enableCrosshair = true;
        
        // vars
        private CharacterController cc;
        private Vector3 vel;
        private Vector3 horizVel; // for weapon stuff
        private bool grounded;
        private float xRot = 0f;
        
        // input stuff
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpPressed;
        private bool wasGroundedLastFrame;
        
        // crosshair system
        private CrosshairManager crosshairManager;
        
        // getters for other scripts
        public bool IsGrounded => grounded;
        public Vector3 Velocity => vel;
        public Vector3 HorizontalVelocity => horizVel;
        public Camera PlayerCamera => cam;
        
        void Awake()
        {
            cc = GetComponent<CharacterController>();
            
            // Initialize velocity to zero
            vel = Vector3.zero;
            horizVel = Vector3.zero;
            
            // find camera if we dont have one
            if (cam == null)
                cam = GetComponentInChildren<Camera>();
            
            if (cam == null)
                cam = Camera.main; // fallback
            
            // ground check setup
            if (groundChecker == null)
            {
                groundChecker = transform.Find("GroundCheck");
                
                if (groundChecker == null)
                {
                    // make one ourselves
                    GameObject gc = new GameObject("GroundCheck");
                    gc.transform.SetParent(transform);
                    gc.transform.localPosition = new Vector3(0, -1f, 0);
                    groundChecker = gc.transform;
                }
            }
                
            Cursor.lockState = CursorLockMode.Locked; // lock mouse
            
            // Setup crosshair
            SetupCrosshair();
        }
        
        void Update()
        {
            GetInput();
            DoMovement();
            DoMouseLook();
        }
        
        void GetInput()
        {
            // get wasd input with dead zone
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            // Apply dead zone to prevent joystick drift
            if (Mathf.Abs(h) < 0.2f) h = 0f;
            if (Mathf.Abs(v) < 0.2f) v = 0f;
            
            // Also check direct key input for more responsive controls
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = Mathf.Max(v, 1f);
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = Mathf.Min(v, -1f);
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = Mathf.Min(h, -1f);
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = Mathf.Max(h, 1f);
            
            moveInput.x = h;
            moveInput.y = v;
            
            // mouse input
            lookInput.x = Input.GetAxis("Mouse X");
            lookInput.y = Input.GetAxis("Mouse Y");
            
            // space to jump
            jumpPressed = Input.GetButtonDown("Jump");
            
            // escape to unlock mouse
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        void DoMovement()
        {
            // store previous grounded state
            wasGroundedLastFrame = grounded;
            
            // check if on ground
            if (groundChecker != null)
            {
                grounded = Physics.CheckSphere(groundChecker.position, groundCheckDist, groundMask);
            }
            else
            {
                grounded = cc.isGrounded; // backup
            }
            
            if (grounded && vel.y < 0)
            {
                vel.y = -2f; // stick to ground
            }
            
            // figure out which way to move
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            
            // walk or run speed
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            horizVel = move * speed;
            cc.Move(horizVel * Time.deltaTime);
            
            // jumping - only allow jump when grounded AND just landed (prevent spam)
            if (jumpPressed && grounded && vel.y <= 0.1f)
            {
                vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // jump formula from physics class
                Debug.Log($"[FPSController] Jump! Velocity Y: {vel.y}, Grounded: {grounded}");
            }
            else if (jumpPressed)
            {
                Debug.Log($"[FPSController] Jump blocked - Grounded: {grounded}, Vel.Y: {vel.y:F2}");
            }
            
            // gravity goes down (only if not grounded or falling)
            if (!grounded || vel.y > 0)
            {
                vel.y += gravity * Time.deltaTime;
            }
            cc.Move(vel * Time.deltaTime);
        }
        
        void DoMouseLook()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;
            if (cam == null) return;
            
            // turn left/right
            transform.Rotate(Vector3.up * lookInput.x * mouseSens);
            
            // look up/down
            xRot -= lookInput.y * mouseSens;
            xRot = Mathf.Clamp(xRot, -maxLookAngle, maxLookAngle);
            cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        }
        
        void SetupCrosshair()
        {
            if (!enableCrosshair) return;
            
            // Add CrosshairManager if it doesn't exist
            crosshairManager = GetComponent<CrosshairManager>();
            if (crosshairManager == null)
            {
                crosshairManager = gameObject.AddComponent<CrosshairManager>();
                Debug.Log("[FPSController] CrosshairManager added automatically");
            }
        }
        
        void OnDrawGizmosSelected()
        {
            // show ground check in editor
            if (groundChecker != null)
            {
                Gizmos.color = grounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundChecker.position, groundCheckDist);
            }
        }
    }
}

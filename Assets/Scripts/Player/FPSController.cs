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
        
        // getters for other scripts
        public bool IsGrounded => grounded;
        public Vector3 Velocity => vel;
        public Vector3 HorizontalVelocity => horizVel;
        public Camera PlayerCamera => cam;
        
        void Awake()
        {
            cc = GetComponent<CharacterController>();
            
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
        }
        
        void Update()
        {
            GetInput();
            DoMovement();
            DoMouseLook();
        }
        
        void GetInput()
        {
            // get wasd input
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            
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
            
            // jumping
            if (jumpPressed && grounded)
            {
                vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // jump formula from physics class
            }
            
            // gravity goes down
            vel.y += gravity * Time.deltaTime;
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

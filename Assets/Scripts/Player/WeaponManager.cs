using UnityEngine;
using SnackAttack.Weapons;

namespace SnackAttack.Player
{
    // handles switching weapons and attacking
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapon Stuff")]
        public BaseWeapon[] weapons;
        public int currentWeaponIdx = 0;
        public Transform weaponHolder;
        
        [Header("Controls")]
<<<<<<< HEAD
        public KeyCode semiAutoKey = KeyCode.F; // F key for semi-auto
        public KeyCode fullAutoKey = KeyCode.G; // G key for full-auto (assault rifle only)
=======
        public KeyCode attackKey = KeyCode.F; // F key for attack
>>>>>>> map_building
        public KeyCode[] weaponKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        
        // stuff we need
        private BaseWeapon currentWeapon;
        private FPSController player;
<<<<<<< HEAD
        private FPSPlayerController playerController; // Support for FPSPlayerController
=======
>>>>>>> map_building
        private float lastUpdateTime;
        
        // getters
        public BaseWeapon CurrentWeapon => currentWeapon;
        public int CurrentWeaponIndex => currentWeaponIdx;
        
        void Awake()
        {
            player = GetComponent<FPSController>();
<<<<<<< HEAD
            playerController = GetComponent<FPSPlayerController>();
            
            // Find camera for weapon holder
            Camera playerCamera = null;
            if (player != null && player.PlayerCamera != null)
            {
                playerCamera = player.PlayerCamera;
            }
            else if (playerController != null)
            {
                // Find camera in FPSPlayerController setup
                playerCamera = GetComponentInChildren<Camera>();
                if (playerCamera == null)
                    playerCamera = Camera.main;
            }
            
            // make weapon holder if we dont have one
            if (weaponHolder == null && playerCamera != null)
            {
                GameObject holder = new GameObject("WeaponHolder");
                holder.transform.SetParent(playerCamera.transform);
=======
            
            // make weapon holder if we dont have one
            if (weaponHolder == null && player != null && player.PlayerCamera != null)
            {
                GameObject holder = new GameObject("WeaponHolder");
                holder.transform.SetParent(player.PlayerCamera.transform);
>>>>>>> map_building
                holder.transform.localPosition = Vector3.zero;
                holder.transform.localRotation = Quaternion.identity;
                weaponHolder = holder.transform;
            }
        }
        
        void Start()
        {
            Debug.Log("[WeaponManager] Start() - Setting up weapons");
            SetupWeapons();
            Debug.Log($"[WeaponManager] Weapons array length: {weapons?.Length ?? 0}");
            SwitchToWeapon(currentWeaponIdx);
            Debug.Log($"[WeaponManager] Current weapon after setup: {(currentWeapon != null ? currentWeapon.WeaponName : "NULL")}");
        }
        
        void Update()
        {
            CheckInput();
            UpdateWeapon();
        }
        
        void CheckInput()
        {
<<<<<<< HEAD
            // Semi-auto attack with F key
            if (Input.GetKeyDown(semiAutoKey))
=======
            // attack with F key
            if (Input.GetKeyDown(attackKey))
>>>>>>> map_building
            {
                Debug.Log("[WeaponManager] F key pressed - calling DoAttack()");
                DoAttack();
            }
            
<<<<<<< HEAD
            // Left mouse click for attack (Unity's default Fire1)
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("[WeaponManager] Mouse click - calling DoAttack()");
                DoAttack();
            }
            
            // Full-auto is handled by individual weapons (G key)
            // AssaultRifleWeapon will handle the G key input directly
            
            // Number keys for weapon switching (1, 2, 3)
=======
            // number keys for weapons
>>>>>>> map_building
            for (int i = 0; i < weaponKeys.Length && i < weapons.Length; i++)
            {
                if (Input.GetKeyDown(weaponKeys[i]))
                {
<<<<<<< HEAD
                    Debug.Log($"[WeaponManager] Number key {i+1} pressed - switching to weapon {i}");
=======
>>>>>>> map_building
                    SwitchToWeapon(i);
                    break;
                }
            }
            
<<<<<<< HEAD
            // Keep scroll wheel for convenience
=======
            // scroll wheel to change weapons
>>>>>>> map_building
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                NextWeapon();
            }
            else if (scroll < 0f)
            {
                PrevWeapon();
            }
        }
        
        void UpdateWeapon()
        {
<<<<<<< HEAD
            if (currentWeapon == null) return;
=======
            if (currentWeapon == null || player == null) return;
>>>>>>> map_building
            
            // Only update animations every few frames to reduce jitter
            if (Time.time - lastUpdateTime < 0.05f) return; // 20 FPS update rate for animations
            lastUpdateTime = Time.time;
            
            // Don't update movement animations during attacks
            if (currentWeapon.IsAttacking) return;
            
            // update weapon animations based on movement
<<<<<<< HEAD
            bool moving = false;
            bool running = false;
            
            if (player != null)
            {
                moving = player.HorizontalVelocity.magnitude > 0.1f;
                running = Input.GetKey(KeyCode.LeftShift) && moving;
            }
            else if (playerController != null)
            {
                // For FPSPlayerController, check input directly
                moving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
                running = Input.GetKey(KeyCode.LeftShift) && moving;
            }
=======
            bool moving = player.HorizontalVelocity.magnitude > 0.1f;
            bool running = Input.GetKey(KeyCode.LeftShift) && moving;
>>>>>>> map_building
            
            currentWeapon.UpdateMovementAnimation(moving, running);
        }
        
        void SetupWeapons()
        {
            // find weapons if we dont have any
            if (weapons == null || weapons.Length == 0)
            {
                weapons = GetComponentsInChildren<BaseWeapon>(true);
            }
            
            // put weapons in the holder
            foreach (BaseWeapon weapon in weapons)
            {
                if (weapon != null && weaponHolder != null)
                {
                    // dont move the arms object itself
                    bool isOnRoot = weapon.transform == transform;
                    
                    if (!isOnRoot)
                    {
                        weapon.transform.SetParent(weaponHolder);
                    }
                    weapon.gameObject.SetActive(false); // hide all weapons first
                }
            }
        }
        
        public void DoAttack()
        {
            if (currentWeapon != null)
            {
                Debug.Log($"[WeaponManager] DoAttack - Current weapon: {currentWeapon.WeaponName}, Can Attack: {currentWeapon.CanAttack}");
                currentWeapon.Attack();
            }
            else
            {
                Debug.LogWarning("[WeaponManager] DoAttack - No current weapon!");
            }
        }
        
        public void SwitchToWeapon(int idx)
        {
            Debug.Log($"[WeaponManager] SwitchToWeapon({idx}) called");
            
            if (weapons == null || idx < 0 || idx >= weapons.Length)
            {
                Debug.LogWarning($"[WeaponManager] Invalid weapon index {idx} or weapons array is null. Array length: {weapons?.Length ?? 0}");
                return;
            }
                
            if (weapons[idx] == null)
            {
                Debug.LogWarning($"[WeaponManager] Weapon at index {idx} is null!");
                return;
            }
            
            Debug.Log($"[WeaponManager] Switching to weapon: {weapons[idx].WeaponName}");
            
            // turn off current weapon
            if (currentWeapon != null)
            {
                Debug.Log($"[WeaponManager] Deactivating current weapon: {currentWeapon.WeaponName}");
                currentWeapon.gameObject.SetActive(false);
            }
            
            // turn on new weapon
            currentWeaponIdx = idx;
            currentWeapon = weapons[currentWeaponIdx];
            currentWeapon.gameObject.SetActive(true);
            
            Debug.Log($"[WeaponManager] Successfully switched to: {currentWeapon.WeaponName}");
        }
        
        public void NextWeapon()
        {
            if (weapons == null || weapons.Length <= 1) return;
            
            int next = (currentWeaponIdx + 1) % weapons.Length;
            SwitchToWeapon(next);
        }
        
        public void PrevWeapon()
        {
            if (weapons == null || weapons.Length <= 1) return;
            
            int prev = (currentWeaponIdx - 1 + weapons.Length) % weapons.Length;
            SwitchToWeapon(prev);
        }
        
        // add new weapon to list
        public void AddWeapon(BaseWeapon weapon)
        {
            if (weapon == null) return;
            
            // make bigger array
            BaseWeapon[] newWeapons = new BaseWeapon[weapons.Length + 1];
            for (int i = 0; i < weapons.Length; i++)
            {
                newWeapons[i] = weapons[i];
            }
            newWeapons[weapons.Length] = weapon;
            weapons = newWeapons;
            
            // setup the weapon
            weapon.transform.SetParent(weaponHolder);
            weapon.gameObject.SetActive(false);
        }
        
        // remove weapon from list
        public void RemoveWeapon(int idx)
        {
            if (weapons == null || idx < 0 || idx >= weapons.Length)
                return;
            
            // switch to different weapon if removing current one
            if (idx == currentWeaponIdx && weapons.Length > 1)
            {
                SwitchToWeapon(idx == 0 ? 1 : 0);
            }
            
            // make smaller array
            BaseWeapon[] newWeapons = new BaseWeapon[weapons.Length - 1];
            int newIdx = 0;
            for (int i = 0; i < weapons.Length; i++)
            {
                if (i != idx)
                {
                    newWeapons[newIdx] = weapons[i];
                    newIdx++;
                }
            }
            weapons = newWeapons;
            
            // fix current weapon index
            if (currentWeaponIdx > idx)
            {
                currentWeaponIdx--;
            }
        }
    }
}

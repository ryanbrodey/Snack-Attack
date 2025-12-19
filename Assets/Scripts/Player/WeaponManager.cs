using UnityEngine;
using SnackAttack.Weapons;

namespace SnackAttack.Player
{
    // handles switching weapons and attacking
    // WARNING: DO NOT use with FPSPlayerControllerWithWeapons - causes input conflicts!
    // Use EITHER WeaponManager OR FPSPlayerControllerWithWeapons, not both!
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapon Stuff")]
        public BaseWeapon[] weapons;
        public int currentWeaponIdx = 0;
        public Transform weaponHolder;

        [Header("Unlockable Weapons")]
        public int pistolIndex = 0;
        public int shotgunIndex = 1;
        public int rifleIndex = 2;

        public bool pistolUnlocked = true;
        public bool shotgunUnlocked = false;
        public bool rifleUnlocked = false;

        [Header("Controls")]
        public KeyCode semiAutoKey = KeyCode.F; // F key for semi-auto
        public KeyCode fullAutoKey = KeyCode.G; // G key for full-auto (assault rifle only)
        public KeyCode[] weaponKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        // stuff we need
        private BaseWeapon currentWeapon;
        private FPSController player;
        private FPSPlayerController playerController; // Support for FPSPlayerController
        private float lastUpdateTime;

        // getters
        public BaseWeapon CurrentWeapon => currentWeapon;
        public int CurrentWeaponIndex => currentWeaponIdx;

        void Awake()
        {
            player = GetComponent<FPSController>();
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
                holder.transform.localPosition = Vector3.zero;
                holder.transform.localRotation = Quaternion.identity;
                weaponHolder = holder.transform;
            }
        }

        void Start()
        {
            SetupWeapons();

            // make sure we start on an unlocked weapon
            if (!IsWeaponUnlocked(currentWeaponIdx))
            {
                if (IsWeaponUnlocked(pistolIndex))
                    currentWeaponIdx = pistolIndex;
                else
                    currentWeaponIdx = 0;
            }

            SwitchToWeapon(currentWeaponIdx);
        }

        void Update()
        {
            CheckInput();
            UpdateWeapon();
        }

        void CheckInput()
        {
            // NOTE: Avoid input conflicts if another controller handles attacks.
            // If you're using FPSPlayerControllerWithWeapons, keep attacks disabled here.

            // Semi-auto attack with F key - DISABLED
            // if (Input.GetKeyDown(semiAutoKey))
            // {
            //     Debug.Log("[WeaponManager] F key pressed - calling DoAttack()");
            //     DoAttack();
            // }

            // Left mouse click for attack - DISABLED
            // if (Input.GetButtonDown("Fire1"))
            // {
            //     Debug.Log("[WeaponManager] Mouse click - calling DoAttack()");
            //     DoAttack();
            // }

            // Number keys for weapon switching (1, 2, 3, 4)
            for (int i = 0; i < weaponKeys.Length && i < weapons.Length; i++)
            {
                if (Input.GetKeyDown(weaponKeys[i]))
                {
                    if (IsWeaponUnlocked(i))
                    {
                        SwitchToWeapon(i);
                    }
                    break;
                }
            }

            // Scroll wheel to change weapons
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
            if (currentWeapon == null || (player == null && playerController == null)) return;

            // Only update animations every few frames to reduce jitter
            if (Time.time - lastUpdateTime < 0.05f) return; // 20 FPS update rate for animations
            lastUpdateTime = Time.time;

            // Don't update movement animations during attacks
            if (currentWeapon.IsAttacking) return;

            // update weapon animations based on movement
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
                currentWeapon.Attack();
            }
        }

        public void SwitchToWeapon(int idx)
        {
            if (weapons == null || idx < 0 || idx >= weapons.Length)
            {
                return;
            }

            if (!IsWeaponUnlocked(idx))
            {
                return;
            }

            if (weapons[idx] == null)
            {
                return;
            }

            // turn off current weapon
            if (currentWeapon != null)
            {
                currentWeapon.gameObject.SetActive(false);
            }

            // turn on new weapon
            currentWeaponIdx = idx;
            currentWeapon = weapons[currentWeaponIdx];
            currentWeapon.gameObject.SetActive(true);
        }

        public void NextWeapon()
        {
            if (weapons == null || weapons.Length <= 1) return;

            int attempts = 0;
            int next = currentWeaponIdx;
            do
            {
                next = (next + 1) % weapons.Length;
                attempts++;
                if (IsWeaponUnlocked(next))
                {
                    SwitchToWeapon(next);
                    return;
                }
            } while (attempts < weapons.Length);
        }

        public void PrevWeapon()
        {
            if (weapons == null || weapons.Length <= 1) return;

            int attempts = 0;
            int prev = currentWeaponIdx;
            do
            {
                prev = (prev - 1 + weapons.Length) % weapons.Length;
                attempts++;
                if (IsWeaponUnlocked(prev))
                {
                    SwitchToWeapon(prev);
                    return;
                }
            } while (attempts < weapons.Length);
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

        // ---------- UNLOCK HELPERS FOR SHOPKEEPER ----------

        private bool IsWeaponUnlocked(int idx)
        {
            if (idx == pistolIndex) return pistolUnlocked;
            if (idx == shotgunIndex) return shotgunUnlocked;
            if (idx == rifleIndex) return rifleUnlocked;

            // any other weapons (if you add more) default to unlocked
            return true;
        }

        public void UnlockShotgun()
        {
            shotgunUnlocked = true;
        }

        public void UnlockRifle()
        {
            rifleUnlocked = true;
        }
    }
}

# Arm Swapping System Setup Guide

## Overview
This system switches between three different arm models (PistolArms, RifleArms, ShotgunArms) when the player changes weapons. Each arm model has its own Animator with its own avatar, so animations work correctly for each weapon.

## How It Works

### 1. Hierarchy Structure
```
FPS_Player_Unified_v2
├── PistolArms (active when pistol equipped)
│   ├── Animator (PistolPlayer_Controller)
│   └── WeaponSocket
│       └── Ketchup (KetchupWeapon script)
├── RifleArms (active when rifle equipped)
│   ├── Animator (RiflePlayer_Controller)
│   └── WeaponSocket
│       └── AssaultRifle (AssaultRifleWeapon script)
├── ShotgunArms (active when shotgun equipped)
│   ├── Animator (ShotgunPlayer_Controller)
│   └── WeaponSocket
│       └── PopcornLauncher (PopcornLauncherWeapon script)
├── CameraAnchor
│   └── Main Camera
└── GroundCheck
```

### 2. Inspector Setup

On `FPS_Player_Unified_v2`, in the `FPSPlayerControllerWithWeapons` component:

**References:**
- **Player Camera**: Main Camera
- **Ground Check**: GroundCheck
- **Camera Anchor**: CameraAnchor

**Arm Models:**
- **Pistol Arms Model**: PistolArms GameObject
- **Rifle Arms Model**: RifleArms GameObject
- **Shotgun Arms Model**: ShotgunArms GameObject

**Weapons Array (Size: 3):**
- **Element 0**: Ketchup (from PistolArms/WeaponSocket/Ketchup)
- **Element 1**: AssaultRifle (from RifleArms/WeaponSocket/AssaultRifle)
- **Element 2**: PopcornLauncher (from ShotgunArms/WeaponSocket/PopcornLauncher)

### 3. What Happens When You Switch Weapons

When you press 1, 2, or 3:

1. **All arm models are deactivated**
   - PistolArms.SetActive(false)
   - RifleArms.SetActive(false)
   - ShotgunArms.SetActive(false)

2. **The correct arm model is activated**
   - Weapon 1 (Pistol) → PistolArms.SetActive(true)
   - Weapon 2 (Rifle) → RifleArms.SetActive(true)
   - Weapon 3 (Shotgun) → ShotgunArms.SetActive(true)

3. **The animator is updated**
   - Gets the Animator component from the active arm model
   - All animation calls (IsWalking, IsRunning, Attack trigger) go to this animator

4. **Camera position is adjusted**
   - Each weapon has its own camera position/rotation
   - The CameraAnchor is moved to the correct position for that weapon

### 4. Camera Positions

These are stored in `InitializeWeaponConfigurations()`:

**Pistol:**
- Position: (-0.199, 1.564, 0.155)
- Rotation: (7.086, -7.197, -0.066)

**Rifle:**
- Position: (-0.004, 1.505, 0.221)
- Rotation: (5.624, -44.278, -0.456)

**Shotgun:**
- Position: (-0.078, 1.542, 0.058)
- Rotation: (5.311, -59.427, 0.353)

### 5. Animation Flow

**Movement Animations:**
- The active arm's animator receives `IsWalking`, `IsRunning`, `IsGrounded`, `IsJumping` parameters
- Each animator controller handles these parameters in its own state machine

**Attack Animations:**
- When you click to shoot, `currentWeapon.Attack()` is called
- The weapon script triggers the "Attack" trigger on its animator
- Each arm model's animator has its own shooting animation

### 6. Key Differences from Unified Animator Approach

**Old Approach (Didn't Work):**
- One animator with sub-state machines for each weapon
- Tried to play rifle/shotgun animations on pistol arms rig → T-pose
- Complex parameter management

**New Approach (Works):**
- Three separate arm models with their own animators
- Each animator uses the correct avatar for its rig
- Simple GameObject activation/deactivation
- Each weapon's animations play correctly on its own rig

## Troubleshooting

**Problem: Weapon model not visible**
- Check that the weapon GameObject is a child of the active arm model's WeaponSocket
- Make sure the weapon's 3D model has correct local position/rotation

**Problem: Animations not playing**
- Check that the active arm model has an Animator component
- Verify the Animator has the correct controller assigned
- Check that the animator parameters exist (IsWalking, IsRunning, Attack)

**Problem: Camera in wrong position**
- Adjust the camera positions in `InitializeWeaponConfigurations()`
- The values are in local space relative to the player

**Problem: Weapon switching not working**
- Make sure all three arm models are assigned in the Inspector
- Check that the weapons array has the correct weapon scripts
- Verify the weapon scripts are on the correct GameObjects

## Controls

- **1, 2, 3**: Switch weapons
- **Left Click or F**: Fire weapon
- **WASD**: Move
- **Shift**: Run
- **Space**: Jump
- **Escape**: Toggle cursor lock


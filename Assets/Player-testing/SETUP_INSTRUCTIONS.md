# FPS Player Setup Instructions

## Overview
I've created an enhanced FPS player system with the following features:
- ✅ WASD movement (W=forward, A=left, S=back, D=right)
- ✅ Double-tap W for auto-run toggle
- ✅ Space bar for realistic jumping (no double jump)
- ✅ Enhanced bullet collision system
- ✅ Improved weapon mechanics with ammo and reload

## Step 1: Setup FPS_Player Prefab

1. **Open the FPS_Player prefab** in the Unity Editor:
   - Navigate to `Assets/Player-testing/Prefabs/FPS_Player.prefab`
   - Double-click to open it in Prefab Mode

2. **Add CharacterController to the root FPS_Player GameObject**:
   - Select the root "FPS_Player" GameObject
   - In the Inspector, click "Add Component"
   - Search for "Character Controller" and add it
   - Set these values:
     - Height: 2
     - Radius: 0.5
     - Center: (0, 1, 0)

3. **Add FPSPlayerController script to the root FPS_Player GameObject**:
   - With FPS_Player still selected, click "Add Component"
   - Search for "FPSPlayerController" and add it
   - Configure the references:
     - **Player Camera**: Drag the "Main Camera (1)" from the hierarchy
     - **Arms Animator**: Drag "PistolArms" from the hierarchy
     - **Ground Check**: Leave empty (will auto-create)
     - **Ground Mask**: Set to "Default" layer or create a "Ground" layer

4. **Update the KetchupGunTest script on PistolArms**:
   - Select the "PistolArms" GameObject in the prefab hierarchy
   - The KetchupGunTest script should already be there
   - Configure these references:
     - **Arms Animator**: Should auto-find (PistolArms)
     - **Bullet Spawn**: Should auto-find (BulletSpawn under Ketchup)
     - **Bullet Prefab**: Drag your bullet prefab here

5. **Save the prefab** by clicking "Save" in the Prefab Mode toolbar

## Step 2: Test in a Scene

1. **Create a test scene or use existing scene**
2. **Drag the FPS_Player prefab** into the scene
3. **Create a ground plane**:
   - Right-click in Hierarchy → 3D Object → Plane
   - Scale it up (e.g., Scale: 10, 1, 10)
   - Make sure it's on the "Default" layer or your Ground layer

4. **Test the controls**:
   - **WASD**: Move around
   - **Double-tap W**: Toggle auto-run
   - **Space**: Jump (realistic physics, no double jump)
   - **Mouse**: Look around
   - **Left Click**: Shoot ketchup bullets
   - **R**: Reload weapon
   - **Tab**: Show debug info
   - **Escape**: Toggle cursor lock

## Step 3: Layer Setup (Optional but Recommended)

1. **Create layers** in Tags & Layers:
   - Player
   - Enemy  
   - Environment
   - Bullet

2. **Set up Physics Layer Collision Matrix**:
   - Edit → Project Settings → Physics
   - Configure bullets to collide with Environment and Enemy layers
   - Prevent bullets from colliding with Player layer

## Controls Summary

| Input | Action |
|-------|--------|
| W | Move Forward |
| A | Move Left |
| S | Move Backward |
| D | Move Right |
| W (double-tap) | Toggle Auto-run |
| Space | Jump |
| Left Shift | Run (while held) |
| Mouse | Look Around |
| Left Click | Shoot |
| R | Reload |
| Tab | Debug Info |
| Escape | Toggle Cursor Lock |

## Features Implemented

### Movement System
- Realistic physics-based movement
- Auto-run system with double-tap W detection
- Proper ground checking and jump physics
- No double jumping allowed
- Smooth mouse look with configurable sensitivity

### Weapon System
- Fire rate limiting
- Ammo system with reload mechanics
- Enhanced bullet collision detection
- Bullets despawn on any collision (realistic behavior)
- Debug logging for troubleshooting

### Animation Integration
- Proper animation parameter updates
- Movement speed calculation
- Running state detection
- Jump state tracking

## Troubleshooting

**If movement doesn't work:**
- Check that CharacterController is added to FPS_Player root
- Verify FPSPlayerController script is attached
- Make sure Ground Check is working (green gizmo when selected)

**If shooting doesn't work:**
- Check bullet prefab is assigned in KetchupGunTest
- Verify BulletSpawn transform exists under Ketchup
- Check console for error messages

**If animations don't play:**
- Verify Arms Animator reference is set
- Check that PistolPlayer_Controller.controller is assigned to PistolArms

**If bullets don't disappear on collision:**
- Make sure bullet prefab has Rigidbody and Collider
- Check that SimpleBullet script is attached to bullet prefab
- Verify collision layers are set up correctly

## Debug Commands

- **Tab**: Show ammo and movement state
- **R**: Force reload (also resets weapon state if stuck)
- Console logs provide detailed information about all systems

The system is now ready to use! All scripts work together to provide a complete FPS experience with realistic movement and weapon mechanics.

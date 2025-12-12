# Unified Weapon Animator Setup Guide

## Overview
This guide explains how to set up a unified animator controller that handles all three weapons (Ketchup Gun, Assault Rifle, Popcorn Shotgun) with a single animator instead of switching between multiple controllers.

## Step 1: Create Unified Animator Controller

1. **Create New Animator Controller**:
   - Right-click in `Assets/Player-testing/Animations/`
   - Create > Animator Controller
   - Name it: `UnifiedWeaponController`

2. **Set Up Parameters**:
   Add these parameters to the animator:
   - `WeaponType` (Int) - 0=Pistol/Ketchup, 1=Rifle, 2=Shotgun
   - `IsWalking` (Bool)
   - `IsRunning` (Bool)
   - `IsGrounded` (Bool)
   - `IsJumping` (Bool)
   - `Attack` (Trigger)
   - `Reload` (Trigger)

## Step 2: Create Animation States

### Base Layer Structure:
```
Entry -> Weapon Type Selector (Sub-State Machine)
├── Pistol Weapon States (Sub-State Machine)
│   ├── Pistol Idle
│   ├── Pistol Walk
│   ├── Pistol Run
│   ├── Pistol Shoot
│   └── Pistol Reload
├── Rifle Weapon States (Sub-State Machine)
│   ├── Rifle Idle
│   ├── Rifle Walk
│   ├── Rifle Run
│   ├── Rifle Shoot
│   └── Rifle Reload
└── Shotgun Weapon States (Sub-State Machine)
    ├── Shotgun Idle
    ├── Shotgun Walk
    ├── Shotgun Run
    ├── Shotgun Shoot
    └── Shotgun Reload
```

## Step 3: Set Up State Machines

### Main State Machine:
1. Create 3 Sub-State Machines:
   - "Pistol States"
   - "Rifle States" 
   - "Shotgun States"

2. **Transitions between weapon types**:
   - From "Any State" to each weapon state machine
   - Condition: `WeaponType` equals 0, 1, or 2
   - Settings: Has Exit Time = false, Transition Duration = 0

### Individual Weapon State Machines:

#### For Each Weapon (Pistol/Rifle/Shotgun):

1. **Idle State** (Entry State):
   - Animation: respective idle animation
   - Transitions:
     - To Walk: `IsWalking == true`
     - To Run: `IsRunning == true`
     - To Attack: `Attack` trigger
     - To Reload: `Reload` trigger

2. **Walk State**:
   - Animation: respective walk animation
   - Transitions:
     - To Idle: `IsWalking == false && IsRunning == false`
     - To Run: `IsRunning == true`
     - To Attack: `Attack` trigger

3. **Run State**:
   - Animation: respective run animation
   - Transitions:
     - To Idle: `IsRunning == false`
     - To Walk: `IsRunning == false && IsWalking == true`
     - To Attack: `Attack` trigger

4. **Attack State**:
   - Animation: respective attack animation
   - Transitions:
     - To Idle: Has Exit Time = true (when animation completes)
   - Animation Events: Call `OnAttackComplete()` at end

5. **Reload State**:
   - Animation: respective reload animation
   - Transitions:
     - To Idle: Has Exit Time = true (when animation completes)
   - Animation Events: Call `OnReloadComplete()` at end

## Step 4: Configure Your Prefab

1. **Add UnifiedWeaponAnimator Component**:
   - Add the `UnifiedWeaponAnimator` script to your FPS_Player_Unified prefab
   - Assign the `UnifiedWeaponController` to the `unifiedController` field
   - Set the `unifiedAnimator` to your arms animator

2. **Update FPSPlayerControllerWithWeapons**:
   - The script is already updated to use the unified system
   - Assign the `UnifiedWeaponAnimator` component to the `unifiedWeaponAnimator` field

## Step 5: Animation Assignments

### Copy animations from existing controllers:
- From `PistolPlayer_Controller.controller`: Get pistol animations
- From `RiflelPlayer_Controller.controller`: Get rifle animations  
- From `ShotgunPlayer_Controller.controller`: Get shotgun animations

### Animation Names to Use:
**Pistol/Ketchup Gun**:
- Idle: "pistol idle"
- Walk: "pistol walk" 
- Run: "pistol run"
- Attack: "Shooting"

**Rifle**:
- Idle: "rifle idle"
- Walk: "rifle walk"
- Run: "rifle run" 
- Attack: "rifle shoot"

**Shotgun**:
- Idle: "shotgun idle"
- Walk: "shotgun walk"
- Run: "shotgun run"
- Attack: "shotgun shoot"

## Step 6: Testing

1. **Weapon Switching**:
   - Press 1: Should switch to Ketchup Gun (WeaponType = 0)
   - Press 2: Should switch to Assault Rifle (WeaponType = 1)
   - Press 3: Should switch to Popcorn Shotgun (WeaponType = 2)

2. **Animation Testing**:
   - Each weapon should have its own idle, walk, run animations
   - Attack animations should play when firing
   - Movement should smoothly transition between states

## Benefits of This Approach

✅ **Single Animator Controller**: No more switching between controllers
✅ **Smooth Transitions**: Better animation blending between weapon types
✅ **Easier Maintenance**: All weapon animations in one place
✅ **Better Performance**: No runtime controller switching overhead
✅ **Scalable**: Easy to add new weapons by adding new sub-state machines

## Troubleshooting

**Issue**: Animations not playing
- Check that WeaponType parameter is being set correctly
- Verify animation clips are assigned to states
- Check transition conditions

**Issue**: Weapon switching not working
- Ensure UnifiedWeaponAnimator component is assigned
- Check that weapon switching calls `SetWeaponType()`
- Verify WeaponType parameter exists in animator

**Issue**: Movement animations not updating
- Check that movement parameters are being updated in UpdateAnimations()
- Verify parameter names match exactly
- Ensure transitions have correct conditions
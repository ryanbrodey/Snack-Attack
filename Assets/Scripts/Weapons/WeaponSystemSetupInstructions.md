# Enhanced Weapon System - Setup Instructions

## Overview
The weapon system has been updated with:
- **UniversalBullet**: Improved bullet physics with automatic cleanup
- **CrosshairAiming**: Accurate shooting that follows crosshair position
- **Enhanced Weapons**: Updated KetchupWeapon, AssaultRifleWeapon, and PopcornLauncherWeapon (now Shotgun)

## Weapon Behaviors

### 1. Ketchup Pistol
- **Fire Mode**: Semi-automatic
- **Fire Rate**: 0.5s between shots
- **Ammo**: 30 rounds
- **Behavior**: Single bullet, crosshair-accurate aiming

### 2. Assault Rifle (Glizzy)
- **Fire Modes**: 
  - Semi-auto: F key or Left Click (0.3s between shots)
  - Full-auto: Hold G key (0.1s between shots)
- **Ammo**: 30 rounds
- **Behavior**: Single bullet, crosshair-accurate aiming, full-auto capability

### 3. Shotgun (PopcornLauncherWeapon)
- **Fire Mode**: Semi-automatic
- **Fire Rate**: 0.8s between shots
- **Ammo**: 8 shells
- **Behavior**: 8 pellets per shot, 15° spread, penetrates 2 enemies per pellet

## Unity Setup Instructions

### Step 1: Update Bullet Prefabs

#### For KetchupBullet Prefab:
1. Add `UniversalBullet` component
2. Configure settings:
   - Speed: 40
   - Damage: 15
   - Lifetime: 5
   - Max Range: 100
   - Can Penetrate: false

#### For GlizzyBullet Prefab (Assault Rifle):
1. Add `UniversalBullet` component
2. Configure settings:
   - Speed: 60
   - Damage: 20
   - Lifetime: 4
   - Max Range: 150
   - Can Penetrate: false

#### For Shotgun Pellet Prefab:
1. Create new prefab or use existing PopcornBullet
2. Add `UniversalBullet` component
3. Configure settings:
   - Speed: 50
   - Damage: 12
   - Lifetime: 3
   - Max Range: 50
   - Can Penetrate: true
   - Max Penetrations: 2

### Step 2: Ensure Bullet Physics

For ALL bullet prefabs, ensure they have:
- **Rigidbody** component:
  - Use Gravity: false
  - Drag: 0
  - Angular Drag: 0
  - Mass: 0.1
- **Collider** component (SphereCollider recommended):
  - Radius: ~0.05
  - Is Trigger: false

### Step 3: Update Weapon GameObjects

#### In your weapon system:
1. **KetchupWeapon**: Should already work with new script
2. **AssaultRifleWeapon**: Should already work with new script
3. **PopcornLauncherWeapon**: 
   - Update `pelletPrefab` field (was `bulletPrefab`)
   - Configure pellet settings in inspector

### Step 4: Layer Setup (Optional but Recommended)

Create these layers for better collision control:
- **Player** (layer 8)
- **Bullet** (layer 9)
- **Enemy** (layer 10)
- **Environment** (layer 11)

Configure Physics Matrix (Edit > Project Settings > Physics):
- Bullets should NOT collide with Player or other Bullets
- Bullets SHOULD collide with Enemies, Environment, Default

### Step 5: Testing

#### Test Each Weapon:
1. **Ketchup Pistol**: 
   - F key or Left Click should fire single bullets
   - Bullets should go where crosshair points
   - Bullets should disappear after 5 seconds or hitting something

2. **Assault Rifle**:
   - F key or Left Click: semi-auto
   - G key (hold): full-auto
   - Should fire rapidly when holding G
   - Bullets should go where crosshair points

3. **Shotgun**:
   - Should fire 8 pellets in spread pattern
   - Pellets should penetrate through enemies
   - Should have slower fire rate than other weapons

## Controls Summary
- **WASD**: Move
- **Mouse**: Look around
- **F Key or Left Click**: Semi-auto fire (all weapons)
- **G Key (hold)**: Full-auto fire (Assault Rifle only)
- **R Key**: Reload
- **1, 2, 3**: Switch weapons
- **Space**: Jump
- **Escape**: Toggle cursor lock

## Troubleshooting

### Bullets Not Firing Correctly:
1. Check that bullet prefabs have `UniversalBullet` component
2. Ensure `bulletSpawn` or `pelletPrefab` is assigned in weapon inspector
3. Check console for debug messages

### Bullets Not Hitting Where Crosshair Points:
1. Ensure player has `CrosshairManager` component
2. Check that camera is properly assigned
3. Look for "No player camera found" warnings in console

### Shotgun Not Spreading:
1. Check `pelletsPerShot` and `spreadAngle` values in inspector
2. Ensure `pelletPrefab` is assigned (not `bulletPrefab`)
3. Check that pellets have `UniversalBullet` component

### Performance Issues:
1. Bullets should auto-destroy after lifetime
2. Check that old bullet prefabs without `UniversalBullet` aren't accumulating
3. Reduce `lifetime` values if needed

## Debug Features

The system includes debug logging and visualization:
- Console messages show bullet firing and hit detection
- `CrosshairAiming.DrawAimDebug()` shows aim lines in Scene view
- Gizmos show bullet range and path in Scene view

Enable Gizmos in Scene view to see bullet trajectories and ranges.


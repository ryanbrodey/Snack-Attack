using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KetchupGunTest : MonoBehaviour
{
    // animator on PistolArms
    public Animator armsAnimator;

    // where bullets spawn (child of Ketchup)
    public Transform bulletSpawn;

    // bullet prefab to spawn
    public GameObject bulletPrefab;

    // speed to launch the bullet
    public float bulletSpeed = 40f;

    void Update()
    {
        // very basic test: left mouse button or Ctrl (Fire1)
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // play shooting animation
        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger("Shoot");
        }

        // spawn a bullet if everything is wired
        if (bulletPrefab != null && bulletSpawn != null)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                bulletSpawn.position,
                bulletSpawn.rotation
            );

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // fire the bullet forward from the bottle tip
                rb.velocity = bulletSpawn.forward * bulletSpeed;
            }
        }
    }
}

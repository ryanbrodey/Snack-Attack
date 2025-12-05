using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleBullet : MonoBehaviour
{
    // how long the bullet lives before cleaning itself up
    public float lifeTime = 3f;

    void Start()
    {
        // destroy after a bit so we do not fill the scene with bullets
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // later we can check tags and do damage
        // for now just destroy the bullet on any hit
        Destroy(gameObject);
    }
}

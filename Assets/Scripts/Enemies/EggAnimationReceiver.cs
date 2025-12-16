using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EggAnimationReceiver : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 10;
    public float hitRadius = 1.2f;
    public Transform attackPoint;   // empty child positioned at the egg's "mouth" or front
    public LayerMask playerMask;

    public void OnAttackHitEvent()
    {
        // simple overlap sphere to detect player
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, hitRadius, playerMask);
        foreach (var hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("Player hit by Egg");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, hitRadius);
    }
}



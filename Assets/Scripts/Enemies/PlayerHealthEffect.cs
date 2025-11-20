using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar; // optional UI reference

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar) healthBar.maxValue = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar) healthBar.value = currentHealth;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // later you can add respawn, game over, etc.
        Debug.Log("Player died");
        gameObject.SetActive(false);
    }
}

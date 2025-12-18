using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;  // 5 hearts
    public int currentHealth;
    
    [Header("Hearts UI")]
    [Tooltip("Drag the 5 heart GameObjects here (in order from left to right)")]
    public GameObject[] hearts;  // Array of heart GameObjects

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        UpdateHeartsUI();
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHeartsUI()
    {
        if (hearts == null || hearts.Length == 0) 
        {
            Debug.LogWarning("[PlayerHealth] Hearts array is empty! Please assign heart GameObjects in the Inspector.");
            return;
        }
        
        // Show/hide hearts based on current health
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                // Show heart if health is greater than this index
                hearts[i].SetActive(i < currentHealth);
            }
        }
    }

    void Die()
    {
        Debug.Log("Player died");
        // Add respawn, game over screen, etc. here
        // For now, just disable the player
        gameObject.SetActive(false);
    }
}

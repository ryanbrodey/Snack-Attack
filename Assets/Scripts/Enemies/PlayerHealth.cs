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
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHeartsUI()
    {
        if (hearts == null || hearts.Length == 0) 
        {
            return;
        }
        
        // Show/hide hearts based on current health
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].SetActive(i < currentHealth);
            }
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}

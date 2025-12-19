using UnityEngine;
using SnackAttack.Weapons;

namespace SnackAttack.Testing
{
    // Test enemy for weapon damage
    public class TestEnemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private float damageFlashDuration = 0.2f;
        
        private Renderer enemyRenderer;
        private bool isFlashing = false;
        
        public float Health => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;
        
        private void Awake()
        {
            currentHealth = maxHealth;
            enemyRenderer = GetComponent<Renderer>();
        }
        
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            // Flash red on damage
            if (enemyRenderer != null && !isFlashing)
            {
                StartCoroutine(FlashDamage());
            }
            
            if (!IsAlive)
            {
                Die();
            }
        }
        
        private System.Collections.IEnumerator FlashDamage()
        {
            isFlashing = true;
            
            if (enemyRenderer != null)
                enemyRenderer.material.color = damageColor;
            
            yield return new WaitForSeconds(damageFlashDuration);
            
            if (enemyRenderer != null)
                enemyRenderer.material.color = normalColor;
            
            isFlashing = false;
        }
        
        private void Die()
        {
            gameObject.SetActive(false);
        }
        
        // Reset enemy to full health
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            gameObject.SetActive(true);
            
            if (enemyRenderer != null)
                enemyRenderer.material.color = normalColor;
        }
    }
}

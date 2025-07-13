using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Efeitos (Opcional)")]
    public GameObject deathEffectPrefab; // Efeito de explosão ou desintegração ao morrer
    public AudioClip deathSound;         // Som de morte
    public AudioSource audioSource;      // Componente AudioSource

    void Start()
    {
        currentHealth = maxHealth;

        // Garante que o AudioSource existe se um som for atribuído
        if (deathSound != null && audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // Garante que a vida não fique negativa

        Debug.Log(gameObject.name + " took " + amount + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " morreu!");

        // Efeitos de morte (som e visual)
        if (deathSound != null && audioSource != null)
        {
            // Toca o som antes de destruir o objeto, para que ele tenha tempo de tocar
            audioSource.PlayOneShot(deathSound);
            // Opcional: desativar o Mesh Renderer e Collider imediatamente para que não seja mais atingido,
            // e só destruir o GameObject depois que o som terminar.
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers) r.enabled = false;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders) c.enabled = false;
            // Destroy(gameObject, deathSound.length); // Destrói depois do som
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }

        // Destrói o inimigo
        Destroy(gameObject);
    }
}
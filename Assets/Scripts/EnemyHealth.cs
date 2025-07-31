using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Death Effects")]
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private Renderer rend;
    private Collider coll;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        rend = GetComponentInChildren<Renderer>();
        coll = GetComponent<Collider>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        if (deathEffectPrefab)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (rend) rend.enabled = false;
        if (coll) coll.enabled = false;

        Destroy(gameObject, 2f); // tempo pra som/efeito terminar
    }
}

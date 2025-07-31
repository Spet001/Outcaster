using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public float speed = 150f;      // Velocidade do projétil
    public float lifeTime = 3f;     // Tempo de vida do projétil antes de ser destruído
    public int damage = 20;         // Dano que o projétil causa (ajuste para pistola/foguete)

    [Header("Efeitos")]
    public GameObject hitEffectPrefab; // Prefab de efeito de impacto (opcional: explosão, faíscas)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerProjectile: Rigidbody component missing on this GameObject!", this);
        }
    }

    void Start()
    {
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * speed; // Move o projétil para a frente (eixo Z local)
        }
        Destroy(gameObject, lifeTime); // Destrói o projétil após seu tempo de vida
    }

    void OnTriggerEnter(Collider other) // Para colisões com Triggers
    {
        // NOTA: Certifique-se de que o Collider do projétil está marcado como "Is Trigger"
        // E que o Rigidbody do projétil tem "Is Kinematic" DESMARCADO

        // Ignorar colisões com o próprio jogador ou outros projéteis do jogador
        // O jogador deve ter a tag "Player"
        // Se houver outros projéteis do jogador que não devem se atingir, eles também podem ter essa tag/layer
        if (other.CompareTag("Player") || other.CompareTag("PlayerProjectile")) // Assumindo uma tag/layer para projéteis do jogador
        {
            return; // Não faz nada se colidir com o jogador ou outro projétil do jogador
        }

        // Se colidir com um inimigo (presumindo que seus inimigos têm a tag "Enemy")
        if (other.CompareTag("Enemy"))
        {
            // Tenta encontrar um script de saúde no inimigo e causar dano
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log("Player Projectile hit Enemy and caused " + damage + " damage!");
            }
        }
        // Você pode adicionar mais condições aqui para colisões com outros objetos (paredes, objetos quebráveis, etc.)
        // if (other.CompareTag("Wall")) { Debug.Log("Hit a wall!"); }


        // Instancia um efeito de impacto (ex: explosão, faíscas) se houver um prefab atribuído
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destrói o projétil após a colisão (a menos que seja o atirador)
        Destroy(gameObject);
    }

    // Se você preferir usar colisões físicas (não triggers)
    void OnCollisionEnter(Collision collision)
    {
        // Se o Collider do projétil NÃO estiver marcado como "Is Trigger",
        // use OnCollisionEnter em vez de OnTriggerEnter.
        // A lógica interna seria similar, mas você acessaria collision.gameObject
        // e collision.collider para obter informações sobre o que foi atingido.
        // Para projéteis que "passam por", OnTriggerEnter é geralmente mais fácil.
    }
}
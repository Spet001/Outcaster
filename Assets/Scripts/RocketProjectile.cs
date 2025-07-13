using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float speed = 100f; // Velocidade do projétil (sugiro valores entre 100-200 para testes)
    public float lifeTime = 5f;  // Tempo de vida do projétil antes de ser destruído
    public int damage = 40;      // Dano que o projétil causa

    private Rigidbody rb; // Referência ao Rigidbody 3D

    void Awake() // Usamos Awake para garantir que o Rigidbody é pego antes de Start
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("RocketProjectile: Rigidbody component missing on this GameObject!", this);
        }
    }

    void Start()
    {
        if (rb != null)
        {
            rb.useGravity = false; // Garante que a gravidade está desativada para o projétil
            rb.linearVelocity = transform.forward * speed; // Move o projétil para a frente (eixo Z local)
        }

        // Destrói o projétil após um tempo, caso não colida com nada
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other) // Método para colisões com triggers 3D
    {
        // Verifica se colidiu com o jogador (baseado na Tag)
        if (other.CompareTag("Player"))
        {
            // Exemplo: pega o componente PlayerStats do jogador e causa dano
            // Certifique-se de que seu script PlayerStats tem um método TakeDamage(int amount)
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
        }
        // Você pode adicionar mais condições aqui para colisões com outras coisas, ex:
        // if (other.CompareTag("Wall")) { /* Tocar som de impacto na parede, etc. */ }
        // if (other.CompareTag("Enemy")) { /* Causa dano em outro inimigo, etc. */ }

        // Destrói o projétil após a colisão
        Destroy(gameObject);
    }
}
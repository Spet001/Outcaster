using UnityEngine;

public class SentryGun : MonoBehaviour
{
    [Header("Referências")]
    public Transform playerTarget;       // O Transform do jogador para a torreta mirar
    public GameObject projectilePrefab; // O prefab do projétil a ser instanciado
    public Transform firePoint;          // O ponto de onde o projétil será disparado

    [Header("Configurações de Disparo")]
    public float fireRate = 1.5f;        // Tempo entre os disparos
    private float nextFireTime;          // O próximo tempo que a torreta poderá disparar

    void Update()
    {
        // Garante que temos um alvo e um ponto de disparo
        if (playerTarget != null && firePoint != null)
        {
            // Faz o FirePoint (e, portanto, a torreta) olhar para o jogador
            firePoint.LookAt(playerTarget.position);

            // Verifica se é hora de disparar
            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + fireRate; // Reseta o timer para o próximo disparo
            }
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            // Instancia o projétil na posição e rotação do FirePoint
            // A rotação do FirePoint já está orientada para o jogador por causa do LookAt
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Projectile Prefab not assigned in SentryGun script!");
        }
    }
}
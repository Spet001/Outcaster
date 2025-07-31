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
    if (playerTarget != null && firePoint != null)
    {
        firePoint.LookAt(playerTarget.position);

        // Raycast para verificar se a sentry realmente vê o player
        Vector3 directionToPlayer = playerTarget.position - firePoint.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, directionToPlayer.normalized, out hit, distanceToPlayer))
        {
            // Verifica se o Raycast atingiu o player
            if (hit.transform == playerTarget)
            {
                if (Time.time >= nextFireTime)
                {
                    FireProjectile();
                    nextFireTime = Time.time + fireRate;
                }
            }
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
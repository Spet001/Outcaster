using UnityEngine;
// REMOVIDO: using System; // Não precisamos mais de Action para OnAmmoChanged

public class WeaponBase : MonoBehaviour
{
    [Header("Configuração Básica da Arma")]
    public string weaponName = "Shotgun"; // Nome para identificar a arma
    public float fireRate = 0.5f;        // Cadência de tiro (segundos entre disparos)
    protected float nextFireTime;        // Próximo tempo que a arma pode atirar

    // --- REMOVIDO: CAMPOS PARA CONTROLE DE MUNIÇÃO ---
    // public int maxAmmo = 30;
    // protected int currentAmmo;
    // --- FIM REMOVIDO ---

    // --- REMOVIDO: EVENTO PARA QUANDO A MUNIÇÃO MUDAR ---
    // public event Action<int, int> OnAmmoChanged;
    // --- FIM REMOVIDO ---

    [Header("Tipo de Disparo")]
    public bool isHitscan = true;        // Define se a arma é hitscan
    public float hitscanRange = 500f;     // Alcance se for hitscan
    public int hitscanDamage = 50;       // Dano se for hitscan

    public GameObject projectilePrefab; // Prefab do projétil (se não for hitscan)
    public int projectileDamage = 40;  // Dano do projétil (se não for hitscan)

    [Header("Referências da Arma")]
    public Transform firePoint;          // O ponto de onde o tiro/raio sai
    public GameObject muzzleFlashPrefab; // Efeito visual de tiro

    [Header("Som da Arma")]
    public AudioClip shootSound;
    protected AudioSource audioSource;   // AudioSource local da arma

    // Para Animação (se tiver um Animator na arma)
    public Animator weaponAnimator;
    public string shootAnimationTrigger = "Shoot"; // Nome do trigger da animação de tiro

    [Header("Configurações de Auto-Aim (Vertical Assist)")]
    public bool enableVerticalAutoAim = true;
    public float autoAimRange = 30f;
    public float autoAimVerticalTolerance = 5f;
    public float autoAimHorizontalAngle = 15f;

    [HideInInspector]
    public Camera playerCamera;

    // --- NOVO: Sprite do Portrait Associado a esta Arma/Estilo ---
    [Header("Configuração de HUD")]
    public Sprite associatedPortrait; // Arraste o sprite do portrait correspondente a esta arma
    // --- FIM NOVO ---

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // --- REMOVIDO: Inicialização e invocação de munição no Awake ---
        // currentAmmo = maxAmmo;
        // OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        // --- FIM REMOVIDO ---

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("WeaponBase: Nenhuma câmera com a tag 'MainCamera' encontrada na cena! O auto-aim vertical não funcionará.", this);
            }
        }
    }

    public void TryFire()
    {
        // --- MODIFICADO: Apenas verifica cadência de tiro (sem munição) ---
        if (Time.time >= nextFireTime)
        {
            // --- REMOVIDO: Decremento de munição e invocação de evento ---
            // currentAmmo--;
            // OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
            // --- FIM REMOVIDO ---

            nextFireTime = Time.time + fireRate;

            if (muzzleFlashPrefab != null)
            {
                GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
                Destroy(flash, 0.1f);
            }
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger(shootAnimationTrigger);
            }

            Vector3 shootDirection = firePoint.forward;
            Transform targetToAimAt = FindAutoAimTarget();

            if (targetToAimAt != null)
            {
                shootDirection = (targetToAimAt.position - firePoint.position).normalized;
                Debug.Log(weaponName + ": Auto-aim ativado! Mirando em: " + targetToAimAt.name);
            }

            if (isHitscan)
            {
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, shootDirection, out hit, hitscanRange))
                {
                    Debug.Log(weaponName + " Hit: " + hit.collider.name + " at " + hit.point);
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(hitscanDamage);
                        }
                    }
                }
                else
                {
                    Debug.Log(weaponName + " Missed!");
                }
            }
            else
            {
                if (projectilePrefab != null)
                {
                    GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

                    PlayerProjectile playerProj = projectileInstance.GetComponent<PlayerProjectile>();
                    if (playerProj != null)
                    {
                        playerProj.damage = projectileDamage;
                    }
                }
                else
                {
                    Debug.LogWarning(weaponName + ": Projectile Prefab not assigned for non-hitscan weapon!");
                }
            }
            // --- REMOVIDO: Debug de munição ---
            // Debug.Log(weaponName + " Current Ammo: " + currentAmmo);
            // --- FIM REMOVIDO ---
        }
        // --- REMOVIDO: Lógica de "No Ammo!" ---
        // else if (currentAmmo <= 0)
        // {
        //     Debug.Log(weaponName + ": No Ammo!");
        //     OnAmmoChanged?.Invoke(0, maxAmmo);
        // }
        // --- FIM REMOVIDO ---
    }

    protected Transform FindAutoAimTarget()
    {
        // ... (o restante do seu método FindAutoAimTarget permanece o mesmo) ...
        if (!enableVerticalAutoAim || playerCamera == null)
        {
            Debug.LogWarning("Auto-aim desativado ou playerCamera é nula. enableVerticalAutoAim: " + enableVerticalAutoAim + ", playerCamera: " + (playerCamera != null ? playerCamera.name : "NULL"));
            return null;
        }

        Debug.Log("FindAutoAimTarget: Buscando inimigos na Layer 'Enemy' a partir de " + playerCamera.transform.position + " com alcance de " + autoAimRange);
        Collider[] hitColliders = Physics.OverlapSphere(playerCamera.transform.position, autoAimRange, LayerMask.GetMask("Enemy"));

        Debug.Log("FindAutoAimTarget: OverlapSphere encontrou " + hitColliders.Length + " colliders.");
        if (hitColliders.Length == 0)
        {
             Debug.LogWarning("FindAutoAimTarget: Nenhum collider encontrado pela OverlapSphere na Layer 'Enemy'. Verifique a Layer e o Collider da torreta!");
        }

        Transform bestTarget = null;
        float closestAngle = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("FindAutoAimTarget: Avaliando collider: " + hitCollider.name + " (Tag: " + hitCollider.tag + ", Layer: " + LayerMask.LayerToName(hitCollider.gameObject.layer) + ")");

            if (!hitCollider.CompareTag("Enemy"))
            {
                Debug.Log("FindAutoAimTarget: Collider " + hitCollider.name + " não tem a Tag 'Enemy', ignorando.");
                continue;
            }

            Vector3 directionToTarget = (hitCollider.transform.position - playerCamera.transform.position).normalized;
            Vector3 cameraForwardFlat = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
            Vector3 directionToTargetFlat = new Vector3(directionToTarget.x, 0, directionToTarget.z).normalized;

            float angleToTarget = Vector3.Angle(cameraForwardFlat, directionToTargetFlat);

            float verticalDifference = Mathf.Abs(hitCollider.transform.position.y - playerCamera.transform.position.y);
            Debug.Log("FindAutoAimTarget: " + hitCollider.name + " - Angle: " + angleToTarget + " (Max: " + autoAimHorizontalAngle + "), Vertical Diff: " + verticalDifference + " (Max: " + autoAimVerticalTolerance + ")");

            if (angleToTarget <= autoAimHorizontalAngle)
            {
                if (verticalDifference <= autoAimVerticalTolerance)
                {
                    if (angleToTarget < closestAngle)
                    {
                        closestAngle = angleToTarget;
                        bestTarget = hitCollider.transform;
                        Debug.Log("FindAutoAimTarget: Novo melhor alvo encontrado: " + bestTarget.name);
                    }
                }
                else
                {
                    Debug.Log("FindAutoAimTarget: " + hitCollider.name + " fora da tolerância vertical.");
                }
            }
            else
            {
                Debug.Log("FindAutoAimTarget: " + hitCollider.name + " fora do ângulo horizontal.");
            }
        }
        Debug.Log("FindAutoAimTarget: Alvo final retornado: " + (bestTarget != null ? bestTarget.name : "NENHUM"));
        return bestTarget;
    }
}
using UnityEngine;

public class WeaponViewportAdjuster : MonoBehaviour
{
    public Camera mainCamera; // Arraste sua MainCamera para cá no Inspector
    public Vector2 viewportPosition = new Vector2(0.5f, 0.1f); // X e Y no Viewport (0 a 1)
    public float distanceFromCamera = 0.5f; // Distância da arma da câmera

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Tenta encontrar a câmera principal
        }
    }

    void LateUpdate() // Use LateUpdate para garantir que a câmera já se moveu
    {
        if (mainCamera == null) return;

        // Converte a posição do viewport (0 a 1) para um ponto no mundo
        Vector3 screenPoint = mainCamera.ViewportToWorldPoint(new Vector3(
            viewportPosition.x,
            viewportPosition.y,
            distanceFromCamera // Z representa a distância da câmera
        ));

        // Posiciona a arma
        transform.position = screenPoint;

        // Opcional: Manter a rotação da arma em relação à câmera
        // Se a arma for filha da câmera, a rotação já será relativa.
        // Se não, você pode querer que ela "olhe" para frente
        // transform.rotation = mainCamera.transform.rotation; 
    }
}

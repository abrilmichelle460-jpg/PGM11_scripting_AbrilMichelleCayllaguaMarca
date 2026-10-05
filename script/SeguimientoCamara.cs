using UnityEngine;

[DisallowMultipleComponent]
public class SeguimientoCamara : MonoBehaviour
{
    [SerializeField] private Transform objetivo;

    private float posicionZInicial;

    private void Awake()
    {
        posicionZInicial = transform.position.z;
    }

    private void LateUpdate()
    {
        if (objetivo == null)
        {
            return;
        }

        // Sigue al jugador en los ejes 2D sin alterar la profundidad de la cámara.
        transform.position = new Vector3(
            objetivo.position.x,
            objetivo.position.y,
            posicionZInicial
        );
    }
}

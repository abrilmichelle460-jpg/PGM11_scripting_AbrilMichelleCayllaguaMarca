using UnityEngine;
using UnityEngine.SceneManagement;

public class PeligroReinicio : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otroColisionador)
    {
        if (otroColisionador.GetComponent<Jugador>() != null)
        {
            ReiniciarNivel();
        }
    }

    // Recarga la escena activa cuando el jugador toca este peligro.
    private void ReiniciarNivel()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }
}

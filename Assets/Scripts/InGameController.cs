using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameController : MonoBehaviour
{
    // Unity llama a Update una vez por frame.
    private void Update()
    {
        // Si el jugador pulsa Escape, volvemos al menú principal.
        if (Input.GetKeyUp(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");
    }
}

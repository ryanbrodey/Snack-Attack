using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuController : MonoBehaviour
{
    [Header("Scene To Reload")]
    [SerializeField] private string gameSceneName = "Map";

    // Called by the YES button
    public void OnYesClicked()
    {
        // Reload the main game scene from the beginning
        SceneManager.LoadScene(gameSceneName);
    }

    // Called by the NO button
    public void OnNoClicked()
    {
        // Intentionally left empty for now.
        // You could add "Application.Quit()" or go back to a main menu later.
    }
}



using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuController : MonoBehaviour
{
    [Header("Scene To Reload")]
    [SerializeField] private string gameSceneName = "Map";

    public void OnYesClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnNoClicked()
    {
        // Could quit or go to main menu here
    }
}



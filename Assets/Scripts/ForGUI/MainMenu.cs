using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string startSceneName = "LevelTest";

    void Awake()
    {
        PlayerPrefs.DeleteAll();
    }

    public void StartGame()
    {
        DataContainer.checkpointIndex = new Vector3(-55, 25, 0);
        SceneManager.LoadScene(startSceneName);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "Loop1";
    
    void Start()
    {
        // Unlock cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    // Called by Start Game button
    public void StartGame()
    {
        // Play click sound
        if (MainMenuAudio.Instance != null)
            MainMenuAudio.Instance.PlayButtonClick();
        
        // Wait a tiny bit so sound plays, then load scene
        Invoke(nameof(LoadGameScene), 0.1f);
    }
    
    void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Called by Exit button
    public void ExitGame()
    {
        // Play click sound
        if (MainMenuAudio.Instance != null)
            MainMenuAudio.Instance.PlayButtonClick();
        
        // Wait a tiny bit so sound plays
        Invoke(nameof(QuitGame), 0.1f);
    }
    
    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
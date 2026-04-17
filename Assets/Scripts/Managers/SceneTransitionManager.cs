using UnityEngine;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Transition Settings")]
    public float fadeDuration = 0.3f;
    public CanvasGroup fadeCanvas;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void TeleportToDoor(Transform player, Transform destination, string nextLoopName = null)
    {
        StartCoroutine(TeleportSequence(player, destination, nextLoopName));
    }
    
    IEnumerator TeleportSequence(Transform player, Transform destination, string nextLoopName)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f));
        
        // Teleport player
        player.position = destination.position;
        player.rotation = destination.rotation;
        
        // Optionally load next loop (if using additive scenes)
        if (!string.IsNullOrEmpty(nextLoopName))
        {
            yield return StartCoroutine(LoadNextLoopAdditive(nextLoopName));
        }
        
        // Fade in
        yield return StartCoroutine(Fade(0f));
    }
    
    IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvas == null) yield break;
        
        float startAlpha = fadeCanvas.alpha;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        
        fadeCanvas.alpha = targetAlpha;
    }
    
    IEnumerator LoadNextLoopAdditive(string sceneName)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        while (!async.isDone)
            yield return null;
        
        // Unload current loop after delay
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
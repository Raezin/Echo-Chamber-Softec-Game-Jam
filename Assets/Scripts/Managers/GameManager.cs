using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // Game state
    public int memoryFragments = 0;
    public int currentLoop = 1;  // 1, 2, or 3
    public int wrongDoorCount = 0;  // For Loop 3 trap
    
    // Settings
    public bool isGameComplete = false;
    
    void Awake()
    {
        // Singleton pattern - only one GameManager exists
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
    
    public void AddMemoryFragment()
    {
        memoryFragments++;
        Debug.Log($"Memory Fragment: {memoryFragments}/3");
        
        // Trigger UI update
        UIManager.Instance?.UpdateFragmentDisplay(memoryFragments);
        
        // Play whisper
        AudioManager.Instance?.PlayWhisper(memoryFragments);
    }
    
    public void IncrementWrongDoor()
    {
        wrongDoorCount++;
        Debug.Log($"Wrong doors: {wrongDoorCount}");
        
        // Update wall counter in Loop 3
        UIManager.Instance?.UpdateWallCounter(wrongDoorCount);
        
        // After 3 failures, spawn mannequins
        if (wrongDoorCount >= 3)
        {
            // Trigger special event
            EventManager.Instance?.TriggerMannequinSpawn();
        }
    }
    
    public void CompleteLoop()
    {
        currentLoop++;
        if (currentLoop > 3)
        {
            isGameComplete = true;
            // Load ending
        }
    }
}
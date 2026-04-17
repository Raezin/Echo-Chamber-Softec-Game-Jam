using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add if using TextMeshPro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Fragment Display")]
    public TextMeshProUGUI fragmentText;  // or regular Text
    public GameObject fragmentIcon;
    
    [Header("Loop Info")]
    public TextMeshProUGUI loopText;
    public TextMeshProUGUI physicsWarningText;
    
    [Header("Wall Counter (Loop 3)")]
    public TextMeshProUGUI wallCounterText;
    public GameObject wallCounterPanel;
    
    [Header("Temporary Messages")]
    public TextMeshProUGUI tempMessageText;
    public float messageDuration = 2f;
    
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
    
    public void UpdateFragmentDisplay(int count)
    {
        if (fragmentText != null)
            fragmentText.text = $"{count}/3";
    }
    
    public void UpdateWallCounter(int count)
    {
        if (wallCounterText != null)
            wallCounterText.text = "#" + count;
    }
    
    public void ShowPhysicsWarning(string warningText, Color color)
    {
        if (physicsWarningText != null)
        {
            physicsWarningText.text = warningText;
            physicsWarningText.color = color;
            Invoke(nameof(HidePhysicsWarning), 3f);
        }
    }
    
    void HidePhysicsWarning()
    {
        if (physicsWarningText != null)
            physicsWarningText.text = "";
    }
    
    public void ShowTempMessage(string message)
    {
        if (tempMessageText != null)
        {
            tempMessageText.text = message;
            CancelInvoke(nameof(HideTempMessage));
            Invoke(nameof(HideTempMessage), messageDuration);
        }
    }
    
    void HideTempMessage()
    {
        if (tempMessageText != null)
            tempMessageText.text = "";
    }
    
    public void SetLoopDisplay(int loopNumber)
    {
        if (loopText != null)
            loopText.text = $"LOOP {loopNumber}";
    }
}
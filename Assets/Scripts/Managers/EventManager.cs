using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    
    // Events for decoupled systems
    public event Action OnGravityFlip;
    public event Action OnTimeDelayStart;
    public event Action OnWrongDoorChosen;
    public event Action<int> OnMannequinSpawn;
    
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
    
    public void TriggerGravityFlip()
    {
        OnGravityFlip?.Invoke();
    }
    
    public void TriggerTimeDelayStart()
    {
        OnTimeDelayStart?.Invoke();
    }
    
    public void TriggerWrongDoor()
    {
        OnWrongDoorChosen?.Invoke();
    }
    
    public void TriggerMannequinSpawn()
    {
        OnMannequinSpawn?.Invoke(3);  // After 3 wrong doors
    }
}
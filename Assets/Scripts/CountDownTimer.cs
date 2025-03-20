using System;
using Unity.Netcode;
using UnityEngine;

public class CountDownTimer : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<int> Seconds = new();
    [HideInInspector] public NetworkVariable<int> Minutes = new();

    public int secondsDuration;
    public int minutesDuration;
    private float elapsed;

    private bool Finished => Minutes.Value == 0 && Seconds.Value == 0;
    
    public event Action OnFinished;

    private void Start()
    {
        Seconds.Value = secondsDuration;
        Minutes.Value = minutesDuration;
    }

    void Update()
    {
        if(!MultiplayerManager.Instance.IsHostAndReady)
            return;
        
        if (Finished || MultiplayerManager.Instance.IsGameOver(out _))
            return;
        
        TakeTime();
        
        if(Finished)
            OnFinished?.Invoke();
    }
    
    void TakeTime()
    {
        elapsed += Time.deltaTime;

        if (elapsed > 1)
        {
            elapsed -= 1;
            Seconds.Value--;
        }

        if (Seconds.Value < 0)
        {
            Seconds.Value = 59;
            Minutes.Value--;
        }
    }
}
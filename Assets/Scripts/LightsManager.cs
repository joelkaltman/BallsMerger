using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using RenderSettings = UnityEngine.RenderSettings;

public class LightsManager : MonoBehaviour
{
    public static LightsManager Instance;
    
    public Light directionalLight;
    public bool Shadows { get; private set; }
    
    void Start()
    {
        Instance = this;
        
        var shadows = PlayerPrefs.GetInt("shadows", 0) == 1;
        if (shadows) {
            this.ShadowsOn ();
        } else {
            this.ShadowsOff ();
        }
    }

    public void ShadowsOn()
    {
        Shadows = true;
        directionalLight.enabled = true;
        RenderSettings.ambientLight = Color.black;
        PlayerPrefs.SetInt("shadows", 1);
    }

    public void ShadowsOff()
    {
        Shadows = false;
        directionalLight.enabled = false;
        RenderSettings.ambientLight = Color.white;
        PlayerPrefs.SetInt("shadows", 0);
    }
}

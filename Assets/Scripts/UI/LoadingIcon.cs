using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIcon : MonoBehaviour
{
    private RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        var rot = rect.eulerAngles;
        rot.z -= 10;
        rect.rotation = Quaternion.Euler(rot);;
    }
}

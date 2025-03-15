using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Attach this script to all the target game objects in the scene.
/// </summary>
[DefaultExecutionOrder(0)]
public class TargetLocator : MonoBehaviour
{
    [Range(0.1f, 1f)]
    [SerializeField] private float screenBoundOffset = 0.9f;
    
    public GameObject indicatorPrefab;

    private GameObject indicator;
    
    private Camera mainCamera;
    private Vector3 screenCentre;
    private Vector3 screenBounds;

    public static event Action<GameObject> OnLocatorSpawn;
    
    void Awake()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
    }
    
    private void Start()
    {
        indicator = Instantiate(indicatorPrefab);
        OnLocatorSpawn?.Invoke(indicator);
    }

    public static void SetLocatorListener(Action<GameObject> listener)
    {
        OnLocatorSpawn = null;
        OnLocatorSpawn += listener;
    }
    
    private void OnDestroy()
    {
        Destroy(indicator.gameObject);
    }

    void Update()
    {
        screenBounds = screenCentre * screenBoundOffset;
        Vector3 screenPosition = GetScreenPosition(mainCamera, transform.position);

        bool visible = IsTargetVisible(screenPosition);
        indicator.SetActive(!visible);
        
        if (visible)
            return;
        
        float angle = float.MinValue;
        GetArrowIndicatorPositionAndAngle(ref screenPosition, ref angle, screenCentre, screenBounds);
        var rotate = indicator.transform.GetChild(0);
        rotate.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        
        indicator.transform.position = screenPosition;
        indicator.transform.localScale = Vector3.one;
    }
    
    public static Vector3 GetScreenPosition(Camera mainCamera, Vector3 targetPosition)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
        return screenPosition;
    }
    
    public static bool IsTargetVisible(Vector3 screenPosition)
    {
        bool isTargetVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;
        return isTargetVisible;
    }

    public static void GetArrowIndicatorPositionAndAngle(ref Vector3 screenPosition, ref float angle, Vector3 screenCentre, Vector3 screenBounds)
    {
        // Our screenPosition's origin is screen's bottom-left corner.
        // But we have to get the arrow's screenPosition and rotation with respect to screenCentre.
        screenPosition -= screenCentre;

        // When the targets are behind the camera their projections on the screen (WorldToScreenPoint) are inverted,
        // so just invert them.
        if(screenPosition.z < 0)
        {
            screenPosition *= -1;
        }

        // Angle between the x-axis (bottom of screen) and a vector starting at zero(bottom-left corner of screen) and terminating at screenPosition.
        angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
        // Slope of the line starting from zero and terminating at screenPosition.
        float slope = Mathf.Tan(angle);

        // Two point's line's form is (y2 - y1) = m (x2 - x1) + c, 
        // starting point (x1, y1) is screen botton-left (0, 0),
        // ending point (x2, y2) is one of the screenBounds,
        // m is the slope
        // c is y intercept which will be 0, as line is passing through origin.
        // Final equation will be y = mx.
        if(screenPosition.x > 0)
        {
            // Keep the x screen position to the maximum x bounds and
            // find the y screen position using y = mx.
            screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
        }
        else
        {
            screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
        }
        // Incase the y ScreenPosition exceeds the y screenBounds 
        if(screenPosition.y > screenBounds.y)
        {
            // Keep the y screen position to the maximum y bounds and
            // find the x screen position using x = y/m.
            screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
        }
        else if(screenPosition.y < -screenBounds.y)
        {
            screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
        }
        // Bring the ScreenPosition back to its original reference.
        screenPosition += screenCentre;
    }
}

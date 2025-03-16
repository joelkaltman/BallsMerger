using UnityEngine;

public class BallsSpawner : MonoBehaviour
{
    [SerializeField] private BallsDataScriptableObject ballsSpawnData;
    [SerializeField] private GameObject guide;
    [SerializeField] private GameObject ball;

    private GameObject ballInstance;
    
#if UNITY_EDITOR
    private bool InputDown => Input.GetMouseButton(0);
    private bool InputReleased => Input.GetMouseButtonUp(0);
    private Vector3 InputPos => new (Input.mousePosition.x, Input.mousePosition.y, 10); 
#else
    private bool InputDown => Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Moved);
    private bool InputReleased => Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Ended;
    private Vector3 InputPos => new (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10); 
#endif

    private Vector3 initPos;

    public void Start()
    {
        initPos = guide.transform.position;
        GenerateBall();
    }

    public void Update()
    {
        if(InputReleased)
            GenerateBall();
        
        guide.SetActive(InputDown);

        var finalPos = initPos;
        if (InputDown)
        {
            var inputWorldPos = Camera.main.ScreenToWorldPoint(InputPos);
            finalPos = new Vector3(inputWorldPos.x, initPos.y, initPos.z);
        }

        guide.transform.position = finalPos;
        ballInstance.transform.position = finalPos;
    }

    private void GenerateBall()
    {
        ballInstance = Instantiate(ball);
        ballInstance.transform.position = initPos;

        var spawnData = ballsSpawnData.GetRandomData();
        ballInstance.transform.localScale = Vector3.one * spawnData.size;
        ballInstance.GetComponent<SpriteRenderer>().color = new Color(spawnData.color.r, spawnData.color.g, spawnData.color.b, 1);
    }
}

using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BallsSpawner : MonoBehaviour
{
    [SerializeField] private BallsDataScriptableObject ballsSpawnData;
    [SerializeField] private GameObject guide;
    [SerializeField] private GameObject limit;
    [SerializeField] private GameObject ball;

    private BallCollision dropBall;
    private List<CollisionData> collisions = new();
    
#if !PLATFORM_ANDROID
    //private bool InputDown => Input.GetMouseButton(0);
    //private bool InputReleased => Input.GetMouseButtonUp(0);
    //private Vector3 InputPos => new (Input.mousePosition.x, Input.mousePosition.y, 10); 
#else
    private bool InputDown => Input.touchCount > 0;
    private bool InputReleased => Input.GetTouch(0).phase == TouchPhase.Ended;
    private Vector3 InputPos => new (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10); 
#endif

    private Vector3 initPos;
    private float limitX;

    public void Start()
    {
        initPos = guide.transform.position;
        limitX = limit.transform.localScale.x * 0.5f;
        CreateDropBall(initPos);
    }

    public void Update()
    {
        ProcessCollisions();

        if (!InputDown)
            return;
        
        var inputWorldPos = Camera.main.ScreenToWorldPoint(InputPos);
        var finalPos = new Vector3(inputWorldPos.x, initPos.y, initPos.z);

        if (finalPos.x < -limitX || finalPos.x > limitX)
            return;
        
        guide.transform.position = finalPos;
        dropBall.transform.position = finalPos;

        if (InputReleased)
        {
            dropBall.Drop();
            CreateDropBall(finalPos);
        }
    }

    private BallCollision CreateBall(BallData data, Vector3 position)
    {
        var instance = Instantiate(ball, transform);
        instance.transform.position = position;

        var ballCol = instance.GetComponent<BallCollision>();
        ballCol.Init(data, limit.transform.position.y);
        ballCol.OnCollision += StackCollision;
        ballCol.OnOutOfBounds += EndGame;

        return ballCol;
    }

    private void CreateDropBall(Vector3 pos)
    {
        var ballData = ballsSpawnData.GetRandomData();
        dropBall = CreateBall(ballData, pos);
    }

    private void StackCollision(CollisionData data)
    {
        if (data.ContainsAny(dropBall.gameObject))
            return;
        
        if(collisions.Any(x => x.ContainsAny(data.ball0, data.ball1)))
            return;
        
        collisions.Add(data);
    }

    private void ProcessCollisions()
    {
        foreach (var collision in collisions)
        {
            var spawnPos = math.lerp(collision.ball0.transform.position, collision.ball1.transform.position, 0.5f);
            
            Destroy(collision.ball0);
            Destroy(collision.ball1);
            
            var localStats = MultiplayerManager.Instance.GetLocalPlayerComponent<PlayerStats>();
            localStats.Score.Value += collision.data.score;

            if (ballsSpawnData.GetNextBall(collision.data.index, out var ballData))
                CreateBall(ballData, spawnPos).Drop();
        }
        collisions.Clear();
    }

    private void EndGame()
    {
        var localStats = MultiplayerManager.Instance.GetLocalPlayerComponent<PlayerStats>();
        localStats.IsDead = true;
    }
}

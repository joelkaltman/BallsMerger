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

    private GameObject dropBall;
    private List<CollisionData> collisions = new();
    
#if UNITY_EDITOR
    private bool InputDown => Input.GetMouseButton(0);
    private bool InputReleased => Input.GetMouseButtonUp(0);
    private Vector3 InputPos => new (Input.mousePosition.x, Input.mousePosition.y, 10); 
#else
    private bool InputDown => Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Moved;
    private bool InputReleased => Input.GetTouch(0).phase == TouchPhase.Ended;
    private Vector3 InputPos => new (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10); 
#endif

    private Vector3 initPos;
    private Vector3 lastPos;

    public void Start()
    {
        initPos = guide.transform.position;
        lastPos = initPos;
    }

    public void Update()
    {
        ProcessCollisions();
        
#if !UNITY_EDITOR
        if (Input.touchCount == 0)
        {
            guide.transform.position = lastPos;
            dropBall.transform.position = lastPos;
            return;
        }
#endif
            
        if(dropBall == null || InputReleased)
            CreateDropBall();

        guide.SetActive(InputDown);
        
        var inputWorldPos = Camera.main.ScreenToWorldPoint(InputPos);
        var finalPos = new Vector3(inputWorldPos.x, initPos.y, initPos.z);
        guide.transform.position = finalPos;
        dropBall.transform.position = finalPos;
        lastPos = finalPos;
    }

    private GameObject CreateBall(BallData data, Vector3 position)
    {
        var instance = Instantiate(ball, transform);
        instance.transform.position = position;

        var ballCol = instance.GetComponent<BallCollision>();
        ballCol.Init(data, limit.transform.position.y);
        ballCol.OnCollision += StackCollision;
        ballCol.OnOutOfBounds += EndGame;

        return instance;
    }

    private void CreateDropBall()
    {
        var ballData = ballsSpawnData.GetRandomData();
        dropBall = CreateBall(ballData, initPos);
    }

    private void StackCollision(CollisionData data)
    {
        if (data.ContainsAny(dropBall))
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
            
            if(ballsSpawnData.GetNextBall(collision.data.index, out var ballData))
                CreateBall(ballData, spawnPos);
        }
        collisions.Clear();
    }

    private void EndGame()
    {
        var localStats = MultiplayerManager.Instance.GetLocalPlayerComponent<PlayerStats>();
        localStats.IsDead = true;
    }
}

using System;
using Unity.Mathematics;
using UnityEngine;

public struct CollisionData
{
    public GameObject ball0;
    public GameObject ball1;
    public BallData data;

    public bool ContainsAny(params GameObject[] balls)
    {
        foreach (var ball in balls)
        {
            if (ball == ball0 || ball == ball1)
                return true;
        }
        return false;
    }
}

public class BallCollision : MonoBehaviour
{
    private const float INITIAL_TIMER = 10f;

    private SpriteRenderer renderer;
    private Rigidbody2D rigidBody;
    private Collider2D collider;
    private float Radius => data.size * 0.5f;
    
    private void ResetTimer() => timerLose = INITIAL_TIMER;
    private void SetColor(Color color) => renderer.color = new Color(color.r, color.g, color.b, 1);

    public event Action<CollisionData> OnCollision;
    public event Action OnOutOfBounds;

    private BallData data;
    private float topY;
    private float timerLose;
    private bool dropped;
    
    public void Init(BallData data, float topY)
    {
        this.data = data;
        this.topY = topY;

        renderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        rigidBody.bodyType = RigidbodyType2D.Static;
        collider.enabled = false;
        
        transform.localScale = Vector3.one * data.size;
        
        ResetTimer();
        SetColor(data.color);
    }

    public void Drop()
    {
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        
        dropped = true;
    }

    private void Update()
    {
        if (!dropped)
            return;
        
        if (timerLose <= 0)
            return;
        
        if (transform.position.y - Radius > topY)
        {
            timerLose -= Time.deltaTime;

            var coef = (INITIAL_TIMER - timerLose) / INITIAL_TIMER;
            var initialColor = new Vector3(data.color.r, data.color.g, data.color.b);
            var color = math.lerp(initialColor, new Vector3(1, 0, 0), coef);
            SetColor(new Color(color.x, color.y, color.z));
            
            if(timerLose <= 0)
                OnOutOfBounds?.Invoke();
        }
        else
        {
            ResetTimer();
            SetColor(data.color);
        }
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!dropped)
            return;
        
        if (!other.gameObject.TryGetComponent(out BallCollision collision))
            return;

        if (collision.data.index != data.index)
            return;

        var colData = new CollisionData()
        {
            ball0 = gameObject,
            ball1 = other.gameObject,
            data = data
        };
        
        OnCollision?.Invoke(colData);
    }
}

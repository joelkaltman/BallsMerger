using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BallsDataScriptableObject", order = 1)]
public class BallsDataScriptableObject : ScriptableObject
{
    public int maxSpawnIndex;
    
    public List<BallData> Datas;

    public BallData GetRandomData()
    {
        var index = Random.Range(0, maxSpawnIndex + 1);
        return Datas[index];
    }

    public bool GetNextBall(int index, out BallData data)
    {
        if (index + 1 < Datas.Count)
        {
            data = Datas[index + 1];
            return true;
        }

        data = default;
        return false;
    }
}
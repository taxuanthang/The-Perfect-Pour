using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    LevelData data;
    
    public LevelData GetLevelData()
    {
        return data; 
    }
}


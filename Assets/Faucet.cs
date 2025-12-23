using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Faucet : MonoBehaviour
{
    [Header("WaterPrefabs")]
    [SerializeField]
    GameObject waterPrefabs;
    [SerializeField]
    bool isPoured = false;
    [SerializeField]
    float createWaterPerSecond = 1.0f;
    [SerializeField]
    Transform waterCreatePos;

    float timeCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Pouring();
    }

    public void Pouring()
    {
        if (!isPoured)
        {
            return;
        }
        if(timeCounter >= createWaterPerSecond)
        {
            timeCounter= 0f;
            Instantiate(waterPrefabs, waterCreatePos.position,Quaternion.identity, waterCreatePos);
        }
        else
        {
            timeCounter += Time.deltaTime;
        }
    }

    public void SetPour(bool isPour)
    {
        this.isPoured = isPour;
    }
}

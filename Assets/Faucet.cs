using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Faucet : MonoBehaviour
{
    [Header("WaterPrefabs")]
    [SerializeField]
    GameObject waterPrefabs;
    [SerializeField]
    GameObject waterDropPrefabs;
    [SerializeField]
    bool isPoured = false;
    [SerializeField]
    float createWaterPerSecond = 1.0f;
    [SerializeField]
    Transform waterCreatePos;
    [SerializeField]
    float count = 0f;
    [SerializeField]
    float randomRange;

    [SerializeField]
    FaucetType faucetType;

    [SerializeField]
    Water water;

    float timeCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
            water.StopPour();
            return;
        }
        //if(timeCounter >= createWaterPerSecond)
        //{
        //    count++;
        //    timeCounter = 0f;
        //    //Vector3 waterPos = waterCreatePos.position + new Vector3(Random.Range(-randomRange, randomRange), 0f,0f);
        //    //Instantiate(waterPrefabs, waterPos, Quaternion.identity, waterCreatePos);
        //}
        //else
        //{
        //    timeCounter += Time.deltaTime;
        //}
        water.Pour();
    }

    public void CreateDelayWater()
    {
        Vector3 waterPos = waterCreatePos.position + new Vector3(Random.Range(-randomRange, randomRange), 0f, 0f);
        Instantiate(waterDropPrefabs, waterPos, Quaternion.identity, waterCreatePos);
    }


    public void SetPour(bool isPour)
    {
        this.isPoured = isPour;
    }
}

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Faucet : MonoBehaviour
{
    [Header("WaterPrefabs")]
    [SerializeField]
    GameObject waterPrefabs;
    [SerializeField]
    GameObject waterDropPrefabs;
    [SerializeField]
    Image waterDropImage;
    [SerializeField]
    bool isPoured = false;

    [Header("Pouring Settings")]

    [Header("FaucetImage")]
    [SerializeField] Image faucetImage;

    [SerializeField]
    Transform waterCreatePos;
    [SerializeField]
    float count = 0f;
    [SerializeField]
    float randomRange;

    [SerializeField]
    FaucetType faucetType;

    [SerializeField]
    WaterType waterType;

    [SerializeField]
    Water water;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    public void CreateWaterFall()
    {
        var waterGO = Instantiate(waterPrefabs, waterCreatePos.position, Quaternion.identity, waterCreatePos);
        water = waterGO.GetComponent<Water>();
        water.SetUp(waterType,faucetType);
    }

    public void SetUp(WaterType waterType, FaucetType faucetType)
    {
        this.faucetType = faucetType;
        this.waterType = waterType;

        CreateWaterFall();
        ChangeFaucetColorBaseOnType();

    }

    public void ChangeFaucetColorBaseOnType()
    {
        switch(faucetType)
        {
            case FaucetType.Normal:
                //change to normal color
                faucetImage.color = Color.white;
                break;
            case FaucetType.X2:
                faucetImage.color = Color.red;
                //change to x2 color
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Pouring();
    }

    public void Pouring()
    {

        if(water == null) return;
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
        GameObject waterDrop = Instantiate(waterDropPrefabs, waterPos, Quaternion.identity, waterCreatePos);
        waterDropImage = waterDrop.GetComponent<Image>();
        // Set water color based on type

            switch (waterType)
            {
                case WaterType.Water:
                    waterDropImage.color = new Color(0.4049484f, 0.9433962f, 0.911468f, 1f);
                    break;
                case WaterType.Milk:
                    waterDropImage.color = Color.white;
                    break;
                case WaterType.Juice:
                    waterDropImage.color = new Color(1f, 0.6092079f, 0f, 1f);
                    break;
                case WaterType.RedWine:
                    waterDropImage.color = Color.red;
                    break;
                case WaterType.Soda:
                    waterDropImage.color = new Color(0.4f, 0.26f, 0.13f, 1f); // Brown
                    break;
                case WaterType.Paint:
                    waterDropImage.color = Color.green;
                    break;

                default:
                    waterDropImage.color = Color.blue;
                    break;
            }
        
    }


    public void SetPour(bool isPour)
    {
        this.isPoured = isPour;
    }
}

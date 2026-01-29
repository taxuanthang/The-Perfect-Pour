using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace Game
{
    public class Bottle : MonoBehaviour
    {
        [SerializeField]
        BottleData data;

        [SerializeField]
        bool isPoured = false;

        [SerializeField]
        Image bottleImage;

        [SerializeField]
        Image mask;

        [SerializeField]
        RectTransform waterTransform;
        [SerializeField]
        Image waterImage;

        [SerializeField]
        RectTransform sodaTransform;

        [SerializeField]
        RectTransform bottleTransform;

        [SerializeField]
        RectTransform goalTransform;

        [Header("goalLevel")]
        [SerializeField]
        GameObject goalSize;
        [SerializeField]
        RectTransform redSize1;

        [SerializeField]
        RectTransform yellowSize1;

        [SerializeField]
        RectTransform greenSize;

        [SerializeField]
        RectTransform yellowSize2;

        [SerializeField]
        RectTransform redSize2;

        [Header("Pouring Settings")]
        [SerializeField]
        float increaseAmount = 0.1f;
        [SerializeField]
        float speed;
        float bottleHeight = 0;
        
        [Header("Foam Settings")]
        [SerializeField]
        float foamDecreaseRate = 0.05f;
        float currentFoamScale = 0f;

        [Header("Lava Settings")]
        [SerializeField]
        float lavaDecreaseRate = 0.05f;
        [SerializeField]
        [Range(0f, 1f)]
        float lavaDecreasePercent = 0.1f; // 100% of droplet increase
        float lavaTargetScale = 0f;
        bool lavaDecreaseStarted = false;
        float dropletIncreaseTotal = 0f;

        public void Awake()
        {
            bottleTransform = GetComponent<RectTransform>();
            waterTransform.localScale = new Vector3(1f,0f,1f);
            if (sodaTransform != null)
            {
                sodaTransform.localScale = new Vector3(1f,0f,1f);
            }
            //ResetGoalLevel
            redSize1.localScale = new Vector3(1f, 0f, 1f);
            yellowSize1.localScale = new Vector3(1f, 0f, 1f);
            greenSize.localScale = new Vector3(1f, 0f, 1f);
            yellowSize2.localScale = new Vector3(1f, 0f, 1f);
            redSize2.localScale = new Vector3(1f, 0f, 1f);
            //Get bottle height
            bottleHeight = bottleTransform.rect.height;
        }

        public void GenerateLevel(BottleData data)
        {
            ResetWaterLevel();

            this.data = data;

            print(data.waterType);


            // Set water color based on type
            if (waterImage != null)
            {
                switch (data.waterType)
                {
                    case WaterType.Water:
                        waterImage.color = new Color(0.4049484f, 0.9433962f, 0.911468f, 1f);
                        break;
                    case WaterType.Milk:
                        waterImage.color = Color.white;
                        break;
                    case WaterType.Juice:
                        waterImage.color = new Color(1f, 0.6092079f, 0f, 1f);
                        break;
                    case WaterType.RedWine:
                        waterImage.color = Color.red;
                        break;
                    case WaterType.Soda:
                        waterImage.color = new Color(0.4f, 0.26f, 0.13f, 1f); // Brown
                        break;
                    case WaterType.Paint:
                        waterImage.color = Color.green;
                        break;

                    default:
                        waterImage.color = Color.blue;
                        break;
                }
            }

            bottleImage.sprite = data.bottle;
            mask.sprite = data.layer;
            // chỉnh goal 

            Vector2 pos = goalTransform.anchoredPosition;
            pos.y = bottleHeight*data.goal;   // giống hệt chỉnh Pos Y trong Inspector
            goalTransform.anchoredPosition = pos;

            // set goalLevel
            redSize1.localScale = new Vector3(1f, data.redSize1, 1f);
            yellowSize1.localScale = new Vector3(1f, data.yellowSize1, 1f);
            greenSize.localScale = new Vector3(1f, data.greenSize, 1f);
            yellowSize2.localScale = new Vector3(1f, data.yellowSize2, 1f);
            redSize2.localScale = new Vector3(1f, data.redSize2, 1f);

            // SET GOAL
            SetGoalActive(false);
            float greenZoneHeight = bottleHeight * (data.greenSize - data.yellowSize2);

            goalTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                greenZoneHeight
            );

            Vector2 goalPos = goalTransform.anchoredPosition;
            goalPos.y = bottleHeight * ((data.greenSize + data.yellowSize2) / 2f);
            goalTransform.anchoredPosition = goalPos;

            // Handle soda foam
            if (sodaTransform != null)
            {
                if (data.waterType == WaterType.Soda)
                {
                    sodaTransform.gameObject.SetActive(true);
                }
                else
                {
                    sodaTransform.gameObject.SetActive(false);
                }
            }
        }

        public void Update()
        {
            IncreaseWaterLevel();
            HandleWaterType();
        }
        public void IncreaseWaterLevel()
        {
            if (!isPoured)
            {
                return;
            }
            foreach (var level in data.listIncreasing)
            {
                if (waterTransform.localScale.y < level.Size)
                {
                    break;
                }
                switch (level.speed)
                {
                    case Speed.Increase:
                        speed = 3f;
                        break;
                    case Speed.Decrease:
                        speed = 1/3f;
                        break;
                }
            }
            if (waterTransform.localScale.y < 1f)
            {
                waterTransform.localScale += new Vector3(0, increaseAmount, 0) * Time.deltaTime * speed;
                
                // Increase foam while actively pouring (1.5x water increase rate)
                if (data.waterType == WaterType.Soda && sodaTransform != null)
                {
                    float waterIncreaseThisFrame = increaseAmount * Time.deltaTime * speed;
                    currentFoamScale += waterIncreaseThisFrame * 1.5f;
                    currentFoamScale = Mathf.Min(currentFoamScale, 1f);
                }
            }
            
        }


        public void HandleWaterType()
        {
            switch (data.waterType)
            {
                case WaterType.Soda:
                    HandleSoda();
                    break;
                case WaterType.Paint:
                    HandleLava();
                    break;
                default:
                    break;
            }
        }

        void HandleSoda()
        {
            if (sodaTransform == null) return;
            
            // Decrease foam after pour is done
            if (!isPoured && currentFoamScale > 0f)
            {
                currentFoamScale -= foamDecreaseRate * Time.deltaTime;
                currentFoamScale = Mathf.Max(currentFoamScale, 0f);
            }
            
            // Update foam visual
            sodaTransform.localScale = new Vector3(1f, currentFoamScale, 1f);
            
            // Keep foam at same bottom position as water
            Vector2 offsetMin = sodaTransform.offsetMin;
            offsetMin.y = waterTransform.offsetMin.y;
            sodaTransform.offsetMin = offsetMin;
        }

        void HandleLava()
        {
            // Slowly decrease water to target after all droplets are done
            if (lavaDecreaseStarted && waterTransform.localScale.y > lavaTargetScale)
            {
                float newScale = waterTransform.localScale.y - lavaDecreaseRate * Time.deltaTime;
                newScale = Mathf.Max(newScale, lavaTargetScale);
                waterTransform.localScale = new Vector3(1f, newScale, 1f);
            }
        }

        public WinState GetWinState()
        {
            float currentLevel = GetCurrentLevel();

            if (data.yellowSize1 <= currentLevel && currentLevel < data.redSize2)
            {
                return WinState.Red;
            }
            else if(data.greenSize <= currentLevel && currentLevel < data.yellowSize1)
            {
                return WinState.Yellow;
            }
            else if(data.yellowSize2 <= currentLevel && currentLevel < data.greenSize)
            {
                return WinState.Green;
            }
            else if(data.redSize2 <= currentLevel && currentLevel < data.yellowSize2)
            {
                return WinState.Yellow;
            }
            else if(0f <= currentLevel && currentLevel < data.redSize2)
            {
                return WinState.Red;
            }
            return WinState.None;
        }

        public float GetCurrentLevel()
        {
            return waterTransform.localScale.y;
        }
       

        public void SetPour(bool isPour)
        {
            this.isPoured = isPour;
        }


        public void CreateDelayWater()
        {
            float dropletIncrease = increaseAmount * 0.3f;
            waterTransform.localScale += new Vector3(0, dropletIncrease, 0);
            
            // Track total droplet increase for lava
            if (data.waterType == WaterType.Paint)
            {
                dropletIncreaseTotal += dropletIncrease;
            }
        }

        public void StartLavaDecrease()
        {
            // Start lava decrease by percentage of total water level
            if (data.waterType == WaterType.Paint && !lavaDecreaseStarted && waterTransform.localScale.y > 0f)
            {
                lavaDecreaseStarted = true;
                float decreaseAmount = waterTransform.localScale.y * lavaDecreasePercent;
                lavaTargetScale = waterTransform.localScale.y - decreaseAmount;
                lavaTargetScale = Mathf.Max(lavaTargetScale, 0f);
            }
        }

        public void ResetWaterLevel()
        {
            waterTransform.localScale = new Vector3(1f, 0f, 1f);
        }

        public void SetGoalActive(bool active)
        {
            goalSize.SetActive(active);
        }
    }
}


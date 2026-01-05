using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace Game
{
    public class Bottle : MonoBehaviour
    {
        [SerializeField]
        BottleData data;

        [SerializeField]
        bool isPoured = false;

        [SerializeField]
        RectTransform waterTransform;

        [SerializeField]
        RectTransform bottleTransform;

        [SerializeField]
        RectTransform goalTransform;

        [Header("goalLevel")]
        [SerializeField]
        GameObject GoalSize;
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
        
        private bool isEffectInProgress = false;
        public UnityEvent OnWaterEffectComplete;

        public void Awake()
        {
            bottleTransform = GetComponent<RectTransform>();
            waterTransform.localScale = new Vector3(1f,0f,1f);
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
            this.data = data;

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

        }
        private bool wasPoured = false;
        public void Update()
        {
            IncreaseWaterLevel();
            if (wasPoured && !isPoured)
            {
                StartCoroutine(ApplyWaterTypeEffect());
            }
            wasPoured = isPoured;
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

        private IEnumerator ApplyWaterTypeEffect()
        {
            isEffectInProgress = true;

            Debug.Log($"Water level before effect: {waterTransform.localScale.y}");

            yield return null;

            float startLevel = waterTransform.localScale.y;
            float targetLevel = startLevel;
            float duration = 1f;

            switch (data.waterType)
            {
                case WaterType.Water:
                    Debug.Log("WaterType: Water (no change)");
                    break;
                case WaterType.Sand:
                    Debug.Log("WaterType: Sand (no change)");
                    break;
                case WaterType.Honey:
                    Debug.Log("WaterType: Honey (no change)");
                    break;
                case WaterType.IceWater:
                    Debug.Log("WaterType: IceWater (increase 5%)");
                    targetLevel = Mathf.Min(startLevel * 1.05f, 1f); // Increase by 5%
                    break;
                case WaterType.Lava:
                    Debug.Log("WaterType: Lava (decrease 8%)");
                    targetLevel = Mathf.Max(startLevel * 0.92f, 0f); // Decrease by 8%
                    break;
                case WaterType.Soda:
                    Debug.Log("WaterType: Soda (wait for foam, decrease 10-15%)");
                    yield return new WaitForSeconds(1.5f);
                    float percent = Random.Range(0.10f, 0.15f);
                    targetLevel = Mathf.Max(startLevel * (1f - percent), 0f); // Decrease by 10-15%
                    break;
            }

            if (!Mathf.Approximately(startLevel, targetLevel))
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    float newY = Mathf.Lerp(startLevel, targetLevel, elapsed / duration);
                    waterTransform.localScale = new Vector3(
                        waterTransform.localScale.x,
                        newY,
                        waterTransform.localScale.z
                    );
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                waterTransform.localScale = new Vector3(
                    waterTransform.localScale.x,
                    targetLevel,
                    waterTransform.localScale.z
                );
            }

            Debug.Log($"Water level after effect: {waterTransform.localScale.y}");
            isEffectInProgress = false;
            OnWaterEffectComplete?.Invoke();
        }
    }
}



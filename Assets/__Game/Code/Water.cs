using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField]
    WaterType waterType;
    [SerializeField]
    Material waterMaterial;
    [SerializeField]
    Image waterImage;

    [SerializeField]
    Rigidbody2D rigidbody2D;
    [SerializeField]
    private float fillSpeed = 1f;
    [SerializeField]
    private float flowSpeed =1;
    private int fillOrigin;
    private float current;
    private float target;

    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }


    public void Start()
    {
        waterImage.fillAmount = 0;
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        switch (waterType)
        {
            case WaterType.Normal:
                fillSpeed = 0.7f;
                flowSpeed = 2f;
                rigidbody2D.gravityScale = 100f;
                break;
            case WaterType.Honey:
                fillSpeed = 0.3f;
                flowSpeed = 1f;
                rigidbody2D.gravityScale = 80;
                break;
            case WaterType.Soda:
                fillSpeed = 0.6f;
                flowSpeed = 2.5f;
                rigidbody2D.gravityScale = 70f;
                break;
            case WaterType.Lava:
                fillSpeed = 0.5f;
                flowSpeed = 1.5f;
                rigidbody2D.gravityScale = 90f;
                break;
        }
    }

    


    public void Pour()
    {
        fillOrigin = 1;
        target = 1f;
        MovingWater();
    }

    bool flag =false;

    public void StopPour()
    {
        if (waterImage.fillAmount <= 0f)
        {
            return;
        }
        if (!flag)
        {
            flag = true;
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }

    }


    public void MovingWater()
    {
        waterImage.fillOrigin = fillOrigin;
        // 1️⃣ Nước dâng / hạ mượt
        current = Mathf.MoveTowards(
            current,
            target,
           fillSpeed *Time.deltaTime
        );
        waterImage.fillAmount = current;

        // 2️⃣ UV scroll giả lập nước chảy
        Vector2 offset = waterMaterial.mainTextureOffset;
        offset += new Vector2(0, flowSpeed * Time.deltaTime);
        waterMaterial.mainTextureOffset = offset;
    }
}

public enum WaterType
{
    Normal,
    Honey,
    Soda,
    Lava

}

public enum FaucetType
{
    Normal,
    Honey,
}

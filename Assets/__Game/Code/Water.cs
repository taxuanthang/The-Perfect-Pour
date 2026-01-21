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
    Sprite waterSprite;
    [SerializeField]
    Sprite milkSprite;
    [SerializeField]
    Sprite juiceSprite;
    [SerializeField]
    Sprite sodaSprite;
    [SerializeField]
    Sprite redwineSprite;
    [SerializeField]
    Sprite paintSprite;

    [SerializeField]
    Rigidbody2D rigidbody2D;
    [SerializeField]
    private float fillSpeed = 1f;
    [SerializeField]
    private float flowSpeed =1;
    private int fillOrigin;
    private float current;
    private float target;

    public bool flag = false;
    [SerializeField]
    private DestroyAfterSeconds destroyAfterSeconds;

    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        destroyAfterSeconds = GetComponent<DestroyAfterSeconds>();
    }


    public void SetUp(WaterType waterType,FaucetType faucetType)
    {
        waterImage.fillAmount = 0;
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        destroyAfterSeconds.enabled = false;
        flag = false;
        waterMaterial.mainTextureOffset = new Vector2(0, 0);

        this.waterType = waterType;
        switch (waterType)
        {
            case WaterType.Water:
                fillSpeed = 0.7f;
                flowSpeed = 2f;
                rigidbody2D.gravityScale = 100f;
                waterMaterial.mainTexture = waterSprite.texture;
                break;
            case WaterType.Milk:
                fillSpeed = 0.3f;
                flowSpeed = 1f;
                rigidbody2D.gravityScale = 80;
                waterMaterial.mainTexture = milkSprite.texture;
                break;
            case WaterType.Juice:
                fillSpeed = 0.3f;
                flowSpeed = 1f;
                rigidbody2D.gravityScale = 80;
                waterMaterial.mainTexture = juiceSprite.texture;
                break;
            case WaterType.Soda:
                fillSpeed = 0.3f;
                flowSpeed = 1f;
                rigidbody2D.gravityScale = 80;
                waterMaterial.mainTexture = sodaSprite.texture;
                break;
            case WaterType.RedWine:
                fillSpeed = 0.3f;
                flowSpeed = 1f;
                rigidbody2D.gravityScale = 80;
                waterMaterial.mainTexture = redwineSprite.texture;
                break;
            case WaterType.Paint:
                fillSpeed = 0.3f;
                flowSpeed = 1f;
                rigidbody2D.gravityScale = 80;
                waterMaterial.mainTexture = paintSprite.texture;
                break;

        }
        switch (faucetType)
        {
            case FaucetType.Normal:
                flowSpeed *= 1;
                break;
            case FaucetType.X2:
                flowSpeed *= 2;
                break;
        }
    }

    


    public void Pour()
    {
        fillOrigin = 1;
        target = 1f;
        MovingWater();
    }

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
            destroyAfterSeconds.enabled = true;
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

    public void Reset()
    {
        flowSpeed = 1f;
    }
}



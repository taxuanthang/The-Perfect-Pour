using UnityEngine;

public static class SpritePadder
{
    public static Sprite PadSprite(
        Sprite original,
        int newWidth,
        int newHeight,
        AnchorMode anchor
    )
    {
        Texture2D src = original.texture;

        Texture2D newTex = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
        newTex.filterMode = FilterMode.Bilinear;

        // Fill toàn bộ = transparent
        Color[] clear = new Color[newWidth * newHeight];
        for (int i = 0; i < clear.Length; i++)
            clear[i] = new Color(0, 0, 0, 0);
        newTex.SetPixels(clear);

        int srcW = (int)original.rect.width;
        int srcH = (int)original.rect.height;

        Color[] srcPixels = src.GetPixels(
            (int)original.rect.x,
            (int)original.rect.y,
            srcW,
            srcH
        );

        Vector2Int pos = GetAnchorPosition(anchor, newWidth, newHeight, srcW, srcH);

        newTex.SetPixels(pos.x, pos.y, srcW, srcH, srcPixels);
        newTex.Apply();

        return Sprite.Create(
            newTex,
            new Rect(0, 0, newWidth, newHeight),
            new Vector2(0.5f, 0.5f),
            original.pixelsPerUnit
        );
    }

    static Vector2Int GetAnchorPosition(AnchorMode mode, int W, int H, int w, int h)
    {
        return mode switch
        {
            AnchorMode.Center => new Vector2Int((W - w) / 2, (H - h) / 2),
            AnchorMode.Top => new Vector2Int((W - w) / 2, H - h),
            AnchorMode.Bottom => new Vector2Int((W - w) / 2, 0),
            AnchorMode.Left => new Vector2Int(0, (H - h) / 2),
            AnchorMode.Right => new Vector2Int(W - w, (H - h) / 2),
            AnchorMode.TopLeft => new Vector2Int(0, H - h),
            AnchorMode.TopRight => new Vector2Int(W - w, H - h),
            AnchorMode.BottomLeft => new Vector2Int(0, 0),
            AnchorMode.BottomRight => new Vector2Int(W - w, 0),
            _ => Vector2Int.zero
        };
    }
}

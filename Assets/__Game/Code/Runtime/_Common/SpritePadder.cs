using UnityEngine;

public static class SpriteScaler
{
    public static Sprite ScaleAndPadSprite(Sprite original, int targetWidth, int targetHeight, AnchorMode anchor)
    {
        // 1. Tính toán tỉ lệ Scale để giữ nguyên Aspect Ratio (Fit)
        float scale = Mathf.Min((float)targetWidth / original.rect.width, (float)targetHeight / original.rect.height);
        int scaledW = Mathf.RoundToInt(original.rect.width * scale);
        int scaledH = Mathf.RoundToInt(original.rect.height * scale);

        // 2. Tạo Texture trung gian đã được scale
        // Chúng ta tạo một bản sao nhỏ hơn/lớn hơn từ vùng chọn của Sprite gốc
        Texture2D scaledTex = CreateScaledTexture(original, scaledW, scaledH);

        // 3. Tạo Texture đích (khung nền trong suốt)
        Texture2D finalTex = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
        Color[] transparentPixels = new Color[targetWidth * targetHeight];
        for (int i = 0; i < transparentPixels.Length; i++) transparentPixels[i] = Color.clear;
        finalTex.SetPixels(transparentPixels);

        // 4. Tính toán vị trí đặt ảnh đã scale vào khung theo Anchor
        Vector2Int pos = GetAnchorPosition(anchor, targetWidth, targetHeight, scaledW, scaledH);

        // 5. Chép pixel từ ảnh đã scale vào ảnh đích
        finalTex.SetPixels(pos.x, pos.y, scaledW, scaledH, scaledTex.GetPixels());
        finalTex.Apply();

        // 6. Tạo Sprite mới
        return Sprite.Create(finalTex, new Rect(0, 0, targetWidth, targetHeight), new Vector2(0.5f, 0.5f), original.pixelsPerUnit);
    }

    private static Texture2D CreateScaledTexture(Sprite sprite, int width, int height)
    {
        // Tạo một RenderTexture tạm thời để thực hiện scale bằng phần cứng
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;

        // Vẽ vùng chọn của Sprite lên RenderTexture
        // Chú ý: Dùng vùng Rect của sprite để hỗ trợ Sprite Sheet
        GL.Clear(true, true, Color.clear);

        // Cách này an toàn nhất cho UI: Vẽ trực tiếp texture lên RT
        Rect sourceRect = new Rect(
            sprite.rect.x / sprite.texture.width,
            sprite.rect.y / sprite.texture.height,
            sprite.rect.width / sprite.texture.width,
            sprite.rect.height / sprite.texture.height
        );

        Graphics.SetRenderTarget(rt);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, 1, 1, 0);
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), sprite.texture, sourceRect, 0, 0, 0, 0);
        GL.PopMatrix();

        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
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
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorWindow : EditorWindow
{
    LevelDatabase database;
    int selectedIndex = 0;
    private Sprite lastBottleSprite;

    [MenuItem("Tools/Level Editor")]
    static void Open()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    void OnGUI()
    {
        DrawDatabaseField();

        if (database == null || database.levels.Count == 0)
            return;

        DrawLevelSelector();

        LevelData level = database.levels[selectedIndex];

        lastBottleSprite = level.bottle;

        DrawLevelData(level);
        DrawPreview(level);
        DrawValidate(level);

        // Khi thay đổi sprite → apply vào bottleTarget của level hiện tại
        if (level.bottle != lastBottleSprite && level.bottle != null)
        {
            ApplyBottleSpriteToTarget(level);
        }
    }

    void DrawDatabaseField()
    {
        EditorGUILayout.Space();
        database = (LevelDatabase)EditorGUILayout.ObjectField(
            "Level Database", database, typeof(LevelDatabase), false);

        if (database != null && GUILayout.Button("Add New Level"))
        {
            CreateLevel();
        }
    }

    void DrawLevelSelector()
    {
        string[] names = new string[database.levels.Count];
        for (int i = 0; i < names.Length; i++)
            names[i] = $"Level {i + 1}";

        selectedIndex = EditorGUILayout.Popup("Select Level", selectedIndex, names);
    }

    void DrawLevelData(LevelData level)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("LEVEL DATA", EditorStyles.boldLabel);

        level.bottle = (Sprite)EditorGUILayout.ObjectField("Bottle Sprite", level.bottle, typeof(Sprite), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("BOTTLE TARGET IN SCENE (Per Level)", EditorStyles.boldLabel);

        GameObject newTarget = (GameObject)EditorGUILayout.ObjectField(
            "Drag Bottle Object Here (từ Hierarchy)",
            level.bottleTarget,
            typeof(GameObject),
            true
        );

        if (newTarget != level.bottleTarget)
        {
            level.bottleTarget = newTarget;
            EditorUtility.SetDirty(level);
        }

        if (level.bottleTarget == null)
        {
            EditorGUILayout.HelpBox("⚠️ Hãy kéo GameObject chai (tên thường là 'Bottle' hoặc 'Goal') từ HIERARCHY vào ô trên.", MessageType.Warning);
        }
        else if (!level.bottleTarget.scene.IsValid() || !level.bottleTarget.scene.isLoaded)
        {
            EditorGUILayout.HelpBox("❌ Bạn kéo PREFAB (màu xanh)! Hãy kéo từ HIERARCHY (scene object).", MessageType.Error);
            level.bottleTarget = null;
            EditorUtility.SetDirty(level);
        }
        else
        {
            EditorGUILayout.HelpBox($"✅ Đã liên kết với: {level.bottleTarget.name} (trong scene)", MessageType.Info);
        }

        EditorGUILayout.Space();
        level.goal = EditorGUILayout.Slider("Goal", level.goal, 0f, 1f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Color Size", EditorStyles.boldLabel);
        level.redSize1 = EditorGUILayout.FloatField("Red 1", level.redSize1);
        level.yellowSize1 = EditorGUILayout.FloatField("Yellow 1", level.yellowSize1);
        level.greenSize = EditorGUILayout.FloatField("Green", level.greenSize);
        level.yellowSize2 = EditorGUILayout.FloatField("Yellow 2", level.yellowSize2);
        level.redSize2 = EditorGUILayout.FloatField("Red 2", level.redSize2);

        EditorGUILayout.Space();
        DrawSpeedIncreasing(level);

        EditorUtility.SetDirty(level);
    }

    void DrawSpeedIncreasing(LevelData level)
    {
        EditorGUILayout.LabelField("Speed Increasing", EditorStyles.boldLabel);

        if (level.listIncreasing == null)
            level.listIncreasing = new System.Collections.Generic.List<SpeedIncreasing>();

        for (int i = 0; i < level.listIncreasing.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            level.listIncreasing[i].speed = (Speed)EditorGUILayout.EnumPopup(level.listIncreasing[i].speed);
            level.listIncreasing[i].Size = EditorGUILayout.Slider(level.listIncreasing[i].Size, 0f, 1f);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                level.listIncreasing.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Speed Rule"))
            level.listIncreasing.Add(new SpeedIncreasing());
    }

    void DrawPreview(LevelData level)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PREVIEW", EditorStyles.boldLabel);

        float previewWidth = 120f;
        float previewHeight = 340f;
        Rect rect = GUILayoutUtility.GetRect(previewWidth, previewHeight, GUILayout.ExpandWidth(false));

        if (level.bottle != null)
        {
            Texture2D tex = level.bottle.texture;
            Rect spriteRect = level.bottle.rect;
            float spriteAspect = spriteRect.width / spriteRect.height;

            float drawWidth = rect.width;
            float drawHeight = rect.height;

            if (drawWidth / drawHeight > spriteAspect)
                drawWidth = drawHeight * spriteAspect;
            else
                drawHeight = drawWidth / spriteAspect;

            Rect drawRect = new Rect(
                rect.x + (rect.width - drawWidth) / 2,
                rect.y + (rect.height - drawHeight) / 2,
                drawWidth,
                drawHeight
            );

            GUI.DrawTextureWithTexCoords(drawRect, tex, new Rect(
                spriteRect.x / tex.width,
                spriteRect.y / tex.height,
                spriteRect.width / tex.width,
                spriteRect.height / tex.height
            ));

            float goalY = drawRect.yMax - (drawHeight * level.goal);
            EditorGUI.DrawRect(new Rect(drawRect.x, goalY - 1f, drawRect.width, 2f), Color.red);
        }
        else
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            EditorGUI.LabelField(rect, "No Sprite", EditorStyles.centeredGreyMiniLabel);
        }
    }

    void DrawValidate(LevelData level)
    {
        if (level.goal <= 0f || level.goal >= 1f)
            EditorGUILayout.HelpBox("Goal phải nằm trong khoảng (0 - 1)", MessageType.Error);

        if (!(level.redSize1 >= level.yellowSize1 &&
              level.yellowSize1 >= level.greenSize &&
              level.greenSize >= level.yellowSize2 &&
              level.yellowSize2 >= level.redSize2))
            EditorGUILayout.HelpBox("Thứ tự size màu bị sai", MessageType.Error);
    }

    void CreateLevel()
    {
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        string path = $"Assets/__Game/Data/Resources/Level_{database.levels.Count + 1}.asset";
        AssetDatabase.CreateAsset(level, path);
        database.levels.Add(level);
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        selectedIndex = database.levels.Count - 1;
    }

    // ===================== ÁP DỤNG SPRITE CHO BOTTLE CỦA LEVEL NÀY =====================
    void ApplyBottleSpriteToTarget(LevelData level)
    {
        if (level.bottleTarget == null)
        {
            Debug.LogWarning($"Level Editor [Level {selectedIndex + 1}]: Chưa kéo Bottle target cho level này.");
            return;
        }

        if (!level.bottleTarget.scene.IsValid() || !level.bottleTarget.scene.isLoaded)
        {
            Debug.LogError($"Level Editor [Level {selectedIndex + 1}]: Bottle target là prefab! Kéo lại từ Hierarchy.");
            return;
        }

        Image img = level.bottleTarget.GetComponent<Image>();
        if (img == null)
            img = level.bottleTarget.GetComponentInChildren<Image>(true);

        if (img == null)
        {
            Debug.LogWarning($"Level Editor [Level {selectedIndex + 1}]: Không tìm thấy Image trên '{level.bottleTarget.name}'");
            return;
        }

        Undo.RecordObject(img, "Apply Bottle Sprite (Per Level)");
        img.sprite = level.bottle;
        EditorUtility.SetDirty(img);
        SceneView.RepaintAll();

        Debug.Log($"Level Editor [Level {selectedIndex + 1}]: Đã thay sprite thành công cho '{img.gameObject.name}' - Kích thước & vị trí giữ nguyên!");
    }
}
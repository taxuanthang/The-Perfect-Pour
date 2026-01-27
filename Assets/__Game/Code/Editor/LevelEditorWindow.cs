using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorWindow : EditorWindow
{
    LevelDatabase database;
    int selectedIndex = 0;
    private Sprite lastBottleSprite;
    private string lastBottleTargetName; // Để detect thay đổi tên

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

        // Cache để detect thay đổi
        if (level.bottle != lastBottleSprite || level.bottleTargetName != lastBottleTargetName)
        {
            ApplyBottleSpriteToScene(level);
            lastBottleSprite = level.bottle;
            lastBottleTargetName = level.bottleTargetName;
        }

        DrawLevelData(level);
        DrawPreview(level);
        DrawValidate(level);

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Sprite to Scene Now (Force)", GUILayout.Height(30)))
        {
            ApplyBottleSpriteToScene(level);
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

        int newIndex = EditorGUILayout.Popup("Select Level", selectedIndex, names);

        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            // Khi chuyển level → force apply ngay sprite của level mới
            LevelData newLevel = database.levels[selectedIndex];
            lastBottleSprite = null; // force trigger apply
            lastBottleTargetName = null;
            ApplyBottleSpriteToScene(newLevel);
        }
    }

    void DrawLevelData(LevelData level)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("LEVEL DATA", EditorStyles.boldLabel);

        level.bottle = (Sprite)EditorGUILayout.ObjectField("Bottle Sprite", level.bottle, typeof(Sprite), false);


        level.layer = (Sprite)EditorGUILayout.ObjectField("Bottle Layer", level.layer, typeof(Sprite), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("BOTTLE IN SCENE", EditorStyles.boldLabel);
        level.bottleTargetName = EditorGUILayout.TextField("Bottle Object Name", level.bottleTargetName);


        level.waterType = (WaterType)EditorGUILayout.EnumPopup("Water Type", level.waterType);
        level.faucetType = (FaucetType)EditorGUILayout.EnumPopup("Faucet Type", level.faucetType);

        EditorGUILayout.HelpBox(
            "Script sẽ tự tìm GameObject có tên chính xác như trên trong scene và thay sprite.\n" +
            "Ví dụ: 'Bottle', 'Goal', 'Bottle_Level2'...\n" +
            "Thay đổi tên → tự apply ngay.",
            MessageType.Info);

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
            level.listIncreasing = new List<SpeedIncreasing>();

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
        string path = $"Assets/__Game/Data/Resources/Level {database.levels.Count + 1}.asset";
        AssetDatabase.CreateAsset(level, path);
        database.levels.Add(level);
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        selectedIndex = database.levels.Count - 1;
    }

    // ===================== ÁP DỤNG SPRITE THEO TÊN =====================
    void ApplyBottleSpriteToScene(LevelData level)
    {
        if (level.bottle == null)
            return;

        if (string.IsNullOrEmpty(level.bottleTargetName))
        {
            Debug.LogWarning($"Level {selectedIndex + 1}: Tên Bottle Object trống!");
            return;
        }

        GameObject go = GameObject.Find(level.bottleTargetName);
        if (go == null)
        {
            Debug.LogWarning($"Level {selectedIndex + 1}: Không tìm thấy GameObject '{level.bottleTargetName}' trong scene!");
            return;
        }

        Image img = go.GetComponent<Image>();
        if (img == null) img = go.GetComponentInChildren<Image>(true);

        if (img == null)
        {
            Debug.LogWarning($"Level {selectedIndex + 1}: Không tìm thấy Image trên '{go.name}'");
            return;
        }

        Undo.RecordObject(img, "Apply Bottle Sprite");
        img.sprite = level.bottle;
        EditorUtility.SetDirty(img);
        SceneView.RepaintAll();

        Debug.Log($"Level Editor: ĐÃ ÁP DỤNG SPRITE cho '{go.name}' (Level {selectedIndex + 1})");
    }
}
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    LevelDatabase database;
    int selectedIndex;

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
        DrawLevelData(database.levels[selectedIndex]);
        DrawPreview(database.levels[selectedIndex]);
        DrawValidate(database.levels[selectedIndex]);
    }

    // =======================

    void DrawDatabaseField()
    {
        database = (LevelDatabase)EditorGUILayout.ObjectField(
            "Level Database",
            database,
            typeof(LevelDatabase),
            false
        );

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

        level.bottle = (Sprite)EditorGUILayout.ObjectField(
            "Bottle Sprite", level.bottle, typeof(Sprite), false);

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
            level.listIncreasing = new();

        for (int i = 0; i < level.listIncreasing.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            level.listIncreasing[i].speed =
                (Speed)EditorGUILayout.EnumPopup(level.listIncreasing[i].speed);

            level.listIncreasing[i].Size =
                EditorGUILayout.Slider(level.listIncreasing[i].Size, 0f, 1f);

            if (GUILayout.Button("X", GUILayout.Width(20)))
                level.listIncreasing.RemoveAt(i);

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Speed Rule"))
            level.listIncreasing.Add(new SpeedIncreasing());
    }

    void DrawPreview(LevelData level)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("PREVIEW", EditorStyles.boldLabel);

        // Kích thước preview (giả lập chiều cao chai)
        Rect rect = GUILayoutUtility.GetRect(80, 240);

        // ===== VẼ CHAI =====
        if (level.bottle != null)
        {
            GUI.DrawTexture(
                rect,
                level.bottle.texture,
                ScaleMode.ScaleToFit
            );
        }
        else
        {
            EditorGUI.DrawRect(rect, Color.black);
        }

        float goalY = rect.yMin + rect.height * level.goal;

        EditorGUI.DrawRect(
            new Rect(rect.x, goalY - 1f, rect.width, 2f),
            Color.red
        );
    }


    void DrawValidate(LevelData level)
    {
        if (level.goal <= 0 || level.goal >= 1)
        {
            EditorGUILayout.HelpBox("Goal phải nằm trong khoảng 0–1", MessageType.Error);
        }

        if (!(level.redSize1 >= level.yellowSize1 &&
              level.yellowSize1 >= level.greenSize &&
              level.greenSize >= level.yellowSize2 &&
              level.yellowSize2 >= level.redSize2))
        {
            EditorGUILayout.HelpBox("Thứ tự size màu bị sai", MessageType.Error);
        }
    }

    void CreateLevel()
    {
        LevelData level = CreateInstance<LevelData>();

        AssetDatabase.CreateAsset(
            level,
            $"Assets/__Game/Data/Resources/Level_{database.levels.Count + 1}.asset"
        );


        database.levels.Add(level);
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
    }
}

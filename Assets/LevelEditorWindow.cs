//using Game;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;
//using static PlasticGui.PlasticTableCell;

//public class LevelEditorWindow : EditorWindow
//{
//    // --- CHUNG ---
//    private LevelConfig currentLevel;
//    private int currentPage = 0; // 0: Box Level, 1: Pig Grid

//    // --- BIẾN CHO BOX LEVEL EDITOR ---
//    private Texture2D sourceImage;
//    private BlockType selectedBlock = BlockType.Black;
//    private Vector2 scrollPosition;
//    private Vector2 leftPanelScroll;
//    private bool useStrictMatching = false;
//    private float zoomScale = 1.0f;
//    private string colorSearchText = "";
//    private bool isResizeMode = false;
    
//    // Bộ lọc màu
//    private float saturationBoost = 1.5f; 
//    private float brightnessBoost = 1.0f; 

//    // --- BIẾN CHO PIG GRID EDITOR ---
//    private int currentPigToolIndex = 0; // 0: Draw, 1: Inspect
//    private int tempPigWidth;
//    private int tempPigHeight;
//    private int bulletAmount = 5;
//    private Texture2D bottleTexture;
//    private Vector2 pigScrollPosition;
//    private Vector2 pigLeftPanelScroll;

//    // --- DATA & HELPER ---
//    public ColorDictionary colorPaletteAsset; 
//    private Dictionary<BlockType, Color32> paletteCache = new Dictionary<BlockType, Color32>();
//    private Dictionary<Color32, BlockType> colorToIdLookup;

//    [MenuItem("Tools/Pixel Flow Level Editor")]
//    public static void ShowWindow()
//    {
//        GetWindow<LevelEditorWindow>("Pixel Flow Editor");
//    }

//    void OnEnable()
//    {
//        RebuildLookupTable();
//        LoadTexture(); // Load icon cái chai cho Pig Editor
//    }

//    void RebuildLookupTable()
//    {
//        if (colorPaletteAsset == null) return;
//        paletteCache.Clear();
//        colorToIdLookup = new Dictionary<Color32, BlockType>(new Color32EqualityComparer());

//        foreach (var map in colorPaletteAsset.colors)
//        {
//            Color32 c32 = (Color32)map.color;
//            if (!paletteCache.ContainsKey(map.type)) paletteCache.Add(map.type, c32);
//            if (!colorToIdLookup.ContainsKey(c32)) colorToIdLookup.Add(c32, map.type);
//        }
//    }

//    void LoadTexture()
//    {
//        // Đường dẫn icon cái chai đạn (Sửa lại nếu cần)
//        string path = "Assets/PixelFlow/Art/Bottle.png"; 
//        bottleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
//    }

//    // ==================================================================================
//    // MAIN GUI
//    // ==================================================================================
//    void OnGUI()
//    {
//        GUILayout.Label("Pixel Flow Level Editor", EditorStyles.boldLabel);

//        // 1. THANH TAB CHUYỂN ĐỔI
//        GUILayout.BeginHorizontal();
//        currentPage = GUILayout.Toolbar(currentPage, new string[] { "📦 Box Level Editor", "🐷 Pig Grid Editor" }, GUILayout.Height(30));
//        GUILayout.EndHorizontal();

//        GUILayout.Space(10);

//        // 2. SLOT CHỌN LEVEL DATA (Dùng chung cho cả 2 tab)
//        GUILayout.BeginHorizontal();
//        var newLevel = (LevelConfig)EditorGUILayout.ObjectField("Level To Edit", currentLevel, typeof(LevelConfig), false);
//        GUILayout.EndHorizontal();

//        // Detect thay đổi level để cập nhật thông số Pig Grid tạm
//        if (newLevel != currentLevel)
//        {
//            currentLevel = newLevel;
//            if (currentLevel != null)
//            {
//                tempPigWidth = currentLevel.pigGridWidth;
//                tempPigHeight = currentLevel.pigGridHeight;
//                // Đảm bảo list PigGrid đủ chỗ
//                currentLevel.pigsList ??= new List<PigData>();
//            }
//        }

//        // Slot chọn Palette (Quan trọng cho cả 2 tab)
//        GUILayout.BeginHorizontal();
//        ColorDictionary newPalette = (ColorDictionary)EditorGUILayout.ObjectField("Color Palette", colorPaletteAsset, typeof(ColorDictionary), false);
//        if (newPalette != colorPaletteAsset) { colorPaletteAsset = newPalette; RebuildLookupTable(); }
//        GUILayout.EndHorizontal();

//        if (currentLevel == null || colorPaletteAsset == null) 
//        {
//            EditorGUILayout.HelpBox("Please assign Level Data & Color Palette!", MessageType.Warning);
//            return;
//        }

//        GUILayout.Space(10);

//        // 3. VẼ GIAO DIỆN THEO TAB
//        if (currentPage == 0)
//        {
//            DrawBoxLevelEditor();
//        }
//        else
//        {
//            DrawPigGridEditor();
//        }

//        if (GUI.changed) EditorUtility.SetDirty(currentLevel);
//    }

//    // ==================================================================================
//    // TAB 1: BOX LEVEL EDITOR (Logic cũ của bạn)
//    // ==================================================================================
//    void DrawBoxLevelEditor()
//    {
//        GUILayout.BeginHorizontal();

//        // --- CỘT TRÁI (TOOLS) ---
//        GUILayout.BeginVertical(GUILayout.Width(280));
//        leftPanelScroll = GUILayout.BeginScrollView(leftPanelScroll);
//        {
//            // A. GRID SETTINGS
//            GUILayout.BeginVertical("box");
//            GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Size:", GUILayout.Width(35));
//            int newWidth = EditorGUILayout.DelayedIntField(currentLevel.boxLevelWidth, GUILayout.Width(40));
//            GUILayout.Label("x", GUILayout.Width(10));
//            int newHeight = EditorGUILayout.DelayedIntField(currentLevel.boxLevelHeight, GUILayout.Width(40));
            
//            if (newWidth != currentLevel.boxLevelWidth || newHeight != currentLevel.boxLevelHeight)
//            {
//                newWidth = Mathf.Clamp(newWidth, 4, 128); 
//                newHeight = Mathf.Clamp(newHeight, 4, 128);
//                ResizeLevelData(newWidth, newHeight);
//            }
//            GUILayout.EndHorizontal();

//            GUILayout.Space(5);
//            GUI.backgroundColor = isResizeMode ? new Color(0.7f, 1f, 0.7f) : Color.white;
//            if (GUILayout.Button(isResizeMode ? "Finish Padding (Done)" : "Edit Grid Padding (+/- Mode)", GUILayout.Height(30)))
//                isResizeMode = !isResizeMode;
//            GUI.backgroundColor = Color.white;
//            if(isResizeMode) EditorGUILayout.HelpBox("Click (+) or (-) buttons around the grid.", MessageType.Info);
//            GUILayout.EndVertical();

//            GUILayout.Space(5);

//            // B. IMG2LEVEL
//            GUILayout.BeginVertical("box");
//            GUILayout.Label("Img2Level Import", EditorStyles.boldLabel);
//            sourceImage = (Texture2D)EditorGUILayout.ObjectField("Image", sourceImage, typeof(Texture2D), false);
            
//            GUILayout.Label("Color Filters:", EditorStyles.miniLabel);
//            saturationBoost = EditorGUILayout.Slider("Saturation", saturationBoost, 0.0f, 3.0f);
//            brightnessBoost = EditorGUILayout.Slider("Brightness", brightnessBoost, 0.5f, 2.0f);
//            if(GUILayout.Button("Reset Filters")) { saturationBoost = 1.5f; brightnessBoost = 1.0f; }

//            useStrictMatching = EditorGUILayout.Toggle("Strict Mode", useStrictMatching);

//            if (sourceImage != null && GUILayout.Button("Generate Level From Image"))
//                ProcessImageToGrid();
//            GUILayout.EndVertical();

//            GUILayout.Space(5);
//            DrawSmartFillTools();
//            GUILayout.Space(5);
//            DrawLevelInfo();
//        }
//        GUILayout.EndScrollView();
//        GUILayout.EndVertical();

//        // --- CỘT PHẢI (GRID VIEWER) ---
//        GUILayout.BeginVertical();
//        {
//            GUILayout.BeginHorizontal(EditorStyles.toolbar);
//            GUILayout.Label("Zoom:", GUILayout.Width(40));
//            zoomScale = GUILayout.HorizontalSlider(zoomScale, 0.2f, 3.0f, GUILayout.Width(200));
//            GUILayout.Label($"{(int)(zoomScale * 100)}%", GUILayout.Width(40));
//            if (GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(50))) zoomScale = 1.0f;
//            GUILayout.FlexibleSpace();
//            GUILayout.EndHorizontal();

//            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, true, true, 
//                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
//            DrawGridWithZoom(); 
            
//            EditorGUILayout.EndScrollView();
//        }
//        GUILayout.EndVertical();
//        GUILayout.EndHorizontal();
//    }

//    // ==================================================================================
//    // TAB 2: PIG GRID EDITOR (Logic từ Recovery.cs)
//    // ==================================================================================
//    void DrawPigGridEditor()
//    {
//        GUILayout.BeginHorizontal();

//        // --- CỘT TRÁI (PIG SETTINGS) ---
//        GUILayout.BeginVertical(GUILayout.Width(280));
//        pigLeftPanelScroll = GUILayout.BeginScrollView(pigLeftPanelScroll);
//        {
//            // A. RESIZE PIG GRID
//            GUILayout.BeginVertical("box");
//            GUILayout.Label("PigGrid Settings", EditorStyles.boldLabel);
//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Size:", GUILayout.Width(40));
//            tempPigWidth = EditorGUILayout.IntField(tempPigWidth, GUILayout.Width(40));
//            GUILayout.Label("x", GUILayout.Width(10));
//            tempPigHeight = EditorGUILayout.IntField(tempPigHeight, GUILayout.Width(40));

//            if (GUILayout.Button("Resize", GUILayout.Width(60)))
//            {
//                Undo.RecordObject(currentLevel, "Resize Pig Level");
//                currentLevel.pigGridWidth = Mathf.Clamp(tempPigWidth, 1, 10);
//                currentLevel.pigGridHeight = Mathf.Clamp(tempPigHeight, 1, 10);
//                currentLevel.InitializePigGrid();
//                // Đảm bảo list PigData đủ số lượng
//                int required = currentLevel.pigGridWidth * currentLevel.pigGridHeight;
//                while (currentLevel.pigsList.Count < required) currentLevel.pigsList.Add(new PigData());
//                EditorUtility.SetDirty(currentLevel);
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.EndVertical();

//            GUILayout.Space(5);

//            // B. PAINTING TOOLS (PIG)
//            GUILayout.BeginVertical("box");
//            GUILayout.Label("Pig Painting Tools", EditorStyles.boldLabel);
            
//            // Searchable Color Picker (Tái sử dụng logic)
//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Search:", GUILayout.Width(50));
//            colorSearchText = EditorGUILayout.TextField(colorSearchText);
//            if (!string.IsNullOrEmpty(colorSearchText) && GUILayout.Button("X", GUILayout.Width(20)))
//            {
//                colorSearchText = "";
//                GUI.FocusControl(null); 
//            }
//            GUILayout.EndHorizontal();

//            if (string.IsNullOrEmpty(colorSearchText))
//            {
//                selectedBlock = (BlockType)EditorGUILayout.EnumPopup("Pen Color:", selectedBlock);
//            }
//            else
//            {
//                DrawSearchableColorList(); // Hàm vẽ list màu tìm kiếm
//            }

//            GUILayout.Space(5);
//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Max Bullet:");
//            bulletAmount = EditorGUILayout.IntField(bulletAmount, GUILayout.Width(40));
//            GUILayout.EndHorizontal();
//            GUILayout.EndVertical();

//            GUILayout.Space(5);

//            // C. STATISTICS (PIG)
//            DrawPigStatistics();
//        }
//        GUILayout.EndScrollView();
//        GUILayout.EndVertical();

//        // --- CỘT PHẢI (PIG GRID VIEWER) ---
//        GUILayout.BeginVertical();
//        pigScrollPosition = EditorGUILayout.BeginScrollView(pigScrollPosition, true, true);
        
//        DrawPigGridVisual(); // Hàm vẽ lưới lợn
        
//        EditorGUILayout.EndScrollView();
//        GUILayout.EndVertical();

//        GUILayout.EndHorizontal();
//    }

//    // ==================================================================================
//    // HELPERS: BOX GRID DRAWING
//    // ==================================================================================
//    void DrawGridWithZoom()
//    {
//        if (currentLevel.boxLevelWidth <= 0 || currentLevel.boxLevelHeight <= 0) return;

//        float baseCellSize = 30f; 
//        float cellSize = baseCellSize * zoomScale;
//        float totalWidth = currentLevel.boxLevelWidth * cellSize;
//        float totalHeight = currentLevel.boxLevelHeight * cellSize;

//        // Container View
//        float viewWidth = position.width - 295f; 
//        float viewHeight = position.height - 45f;
//        float margin = isResizeMode ? 60f : 0f;
//        float containerW = Mathf.Max(totalWidth + margin * 2, viewWidth);
//        float containerH = Mathf.Max(totalHeight + margin * 2, viewHeight);

//        Rect containerRect = GUILayoutUtility.GetRect(containerW, containerH);
//        EditorGUI.DrawRect(containerRect, new Color(0.15f, 0.15f, 0.15f)); 

//        float startX = containerRect.x + (containerW - totalWidth) / 2f;
//        float startY = containerRect.y + (containerH - totalHeight) / 2f;
//        Rect gridRect = new Rect(startX, startY, totalWidth, totalHeight);

//        // Vẽ nút (+)(-) nếu ở chế độ Resize
//        if (isResizeMode)
//        {
//            float btnSize = 20f; float btnSpacing = 2f;
//            DrawResizeButtons(new Rect(gridRect.center.x - btnSize, gridRect.y - btnSize - 5, btnSize, btnSize), new Rect(gridRect.center.x + btnSpacing, gridRect.y - btnSize - 5, btnSize, btnSize), () => InsertRow(0), () => RemoveRow(0));
//            DrawResizeButtons(new Rect(gridRect.center.x - btnSize, gridRect.yMax + 5, btnSize, btnSize), new Rect(gridRect.center.x + btnSpacing, gridRect.yMax + 5, btnSize, btnSize), () => InsertRow(currentLevel.boxLevelHeight), () => RemoveRow(currentLevel.boxLevelHeight - 1));
//            DrawResizeButtons(new Rect(gridRect.x - btnSize - 5, gridRect.center.y - btnSize, btnSize, btnSize), new Rect(gridRect.x - btnSize - 5, gridRect.center.y + btnSpacing, btnSize, btnSize), () => InsertColumn(0), () => RemoveColumn(0));
//            DrawResizeButtons(new Rect(gridRect.xMax + 5, gridRect.center.y - btnSize, btnSize, btnSize), new Rect(gridRect.xMax + 5, gridRect.center.y + btnSpacing, btnSize, btnSize), () => InsertColumn(currentLevel.boxLevelWidth), () => RemoveColumn(currentLevel.boxLevelWidth - 1));
//        }

//        EditorGUI.DrawRect(gridRect, Color.black);

//        for (int y = 0; y < currentLevel.boxLevelHeight; y++)
//        {
//            for (int x = 0; x < currentLevel.boxLevelWidth; x++)
//            {
//                // Culling view
//                if (startX + x * cellSize > scrollPosition.x + viewWidth || startX + (x+1) * cellSize < scrollPosition.x ||
//                    startY + y * cellSize > scrollPosition.y + viewHeight || startY + (y+1) * cellSize < scrollPosition.y)
//                    continue;

//                int index = y * currentLevel.boxLevelWidth + x;
//                if (index >= currentLevel.boxsList.Count) continue;

//                BlockType type = currentLevel.boxsList[index].colorType;
//                Rect cellRect = new Rect(gridRect.x + x * cellSize, gridRect.y + y * cellSize, cellSize - 1, cellSize - 1);

//                Color drawColor = Color.gray; 
//                if (paletteCache.ContainsKey(type)) drawColor = (Color)paletteCache[type];
//                else if (type == BlockType.Empty) drawColor = new Color(0.2f, 0.2f, 0.2f);

//                EditorGUI.DrawRect(cellRect, drawColor);
//            }
//        }

//        if (!isResizeMode)
//        {
//            Event e = Event.current;
//            if (gridRect.Contains(e.mousePosition))
//            {
//                if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
//                {
//                    int gx = Mathf.FloorToInt((e.mousePosition.x - gridRect.x) / cellSize);
//                    int gy = Mathf.FloorToInt((e.mousePosition.y - gridRect.y) / cellSize);
//                    gx = Mathf.Clamp(gx, 0, currentLevel.boxLevelWidth - 1);
//                    gy = Mathf.Clamp(gy, 0, currentLevel.boxLevelHeight - 1);
//                    int idx = gy * currentLevel.boxLevelWidth + gx;

//                    if (e.button == 0) currentLevel.boxsList[idx].colorType = selectedBlock;     
//                    else if (e.button == 1) currentLevel.boxsList[idx].colorType = BlockType.Empty; 

//                    e.Use();
//                    Repaint();
//                }
//            }
//        }
//    }

//    // ==================================================================================
//    // HELPERS: PIG GRID DRAWING
//    // ==================================================================================
//    void DrawPigGridVisual()
//    {
//        if (currentLevel.pigGridWidth <= 0 || currentLevel.pigGridHeight <= 0) return;

//        float cellSize = 80f;
//        int gridWidth = currentLevel.pigGridWidth;
//        int gridHeight = currentLevel.pigGridHeight;
//        float totalWidth = gridWidth * cellSize;
//        float totalHeight = gridHeight * cellSize;

//        Rect gridRect = GUILayoutUtility.GetRect(totalWidth, totalHeight);
//        EditorGUI.DrawRect(gridRect, new Color(0.1f, 0.1f, 0.1f));

//        GUIStyle centered = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12, fontStyle = FontStyle.Bold };
//        float borderThickness = 2f;

//        for (int y = 0; y < gridHeight; y++)
//        {
//            for (int x = 0; x < gridWidth; x++)
//            {
//                int index = y * gridWidth + x;
//                if (index >= currentLevel.pigsList.Count) continue;

//                BlockType type = currentLevel.pigsList[index].colorType;
//                int bullets = currentLevel.pigsList[index].maxBullets;

//                Rect cellBase = new Rect(gridRect.x + x * cellSize, gridRect.y + y * cellSize, cellSize, cellSize);
                
//                // Vẽ viền
//                DrawBorder(cellBase, Color.gray, borderThickness);

//                // Vẽ nội dung (Icon + Màu)
//                Rect contentRect = new Rect(cellBase.x + borderThickness, cellBase.y + borderThickness, cellSize - borderThickness*2, cellSize - borderThickness*2);
                
//                if (type != BlockType.Empty && paletteCache.ContainsKey(type))
//                {
//                    Color c = paletteCache[type];
//                    GUI.color = c; // Tint màu cho texture
//                    // Vẽ Texture (Chai nước) nếu có, hoặc vẽ ô vuông màu
//                    if (bottleTexture != null) GUI.DrawTexture(contentRect, bottleTexture);
//                    else EditorGUI.DrawRect(contentRect, c);
//                    GUI.color = Color.white; // Reset
//                }
//                else
//                {
//                    EditorGUI.DrawRect(contentRect, new Color(0.2f,0.2f,0.2f));
//                }

//                // Vẽ số đạn
//                GUI.Label(contentRect, bullets.ToString(), centered);
//            }
//        }

//        // Input Chuột cho Pig Grid
//        Event e = Event.current;
//        if (gridRect.Contains(e.mousePosition))
//        {
//            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
//            {
//                int gx = Mathf.FloorToInt((e.mousePosition.x - gridRect.x) / cellSize);
//                int gy = Mathf.FloorToInt((e.mousePosition.y - gridRect.y) / cellSize);
//                int idx = gy * gridWidth + gx;

//                if (idx >= 0 && idx < currentLevel.pigsList.Count)
//                {
//                    if (e.button == 0) // Trái: Gán màu + đạn
//                    {
//                        currentLevel.pigsList[idx].colorType = selectedBlock;
//                        currentLevel.pigsList[idx].maxBullets = bulletAmount;
//                    }
//                    else if (e.button == 1) // Phải: Xóa
//                    {
//                        currentLevel.pigsList[idx].colorType = BlockType.Empty;
//                        currentLevel.pigsList[idx].maxBullets = 0;
//                    }
//                    e.Use();
//                    Repaint();
//                }
//            }
//        }
//    }

//    void DrawPigStatistics()
//    {
//               #region C. Thông số mà phải vẽ
//        GUILayout.BeginVertical("box");

//        if (currentLevel != null)
//        {
//            // Gọi Helper để lấy số liệu phân tích
//            var stats = LevelInfo.AnalyzeLevel(currentLevel);

//            List<BlockType> colorsToFind = stats
//                .Where(s => s.count > 0)
//                .Select(s => s.type)
//                .ToList();
//            var pigGridStats = PigGridAnalyseInfo.AnalyzeColorInPigInLevel(currentLevel, colorsToFind);

//            GUILayout.Space(5);
//            GUILayout.Label($"Total Pixels: {currentLevel.boxLevelWidth * currentLevel.boxLevelHeight}", EditorStyles.miniLabel);

//            GUILayout.Space(5);
//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Tên màu", GUILayout.Width(100));
//            GUILayout.Label("Count", GUILayout.Width(40));
//            GUILayout.Label("Drawed", GUILayout.Width(60));
//            GUILayout.Label("Left", GUILayout.Width(40));
//            GUILayout.EndHorizontal();

//            GUILayout.Space(5);
//            for (int i = 0; i < stats.Count; i++)
//            {
//                if (stats[i].count == 0) continue; // Không hiện màu không dùng

//                GUILayout.BeginHorizontal();

//                // 1. Vẽ ô màu nhỏ (Visual)
//                Color32 c = new Color32(0, 0, 0, 0);
//                if (paletteCache.ContainsKey(stats[i].type)) c = paletteCache[stats[i].type];
//                else if (stats[i].type == BlockType.Empty) c = new Color32(50, 50, 50, 255); // Màu xám cho Empty

//                Rect colorRect = GUILayoutUtility.GetRect(15, 15, GUILayout.Width(15));
//                EditorGUI.DrawRect(colorRect, c);

//                // 2. Tên và Số lượng
//                GUILayout.Label($"{stats[i].type}", GUILayout.Width(100));
//                GUILayout.Label($"{stats[i].count}", GUILayout.Width(40));
//                GUILayout.Label($"{pigGridStats[i].count}", GUILayout.Width(60));
//                GUILayout.Label($"{stats[i].count - pigGridStats[i].count}", GUILayout.Width(40));

//                GUILayout.EndHorizontal();
//            }
//        }
//        GUILayout.EndVertical();
//        #endregion
//    }

//    // ==================================================================================
//    // CÁC HÀM TIỆN ÍCH (HELPER)
//    // ==================================================================================
//    void DrawSmartFillTools()
//    {
//        GUILayout.BeginVertical("box");
//        GUILayout.Label("Painting Tools", EditorStyles.boldLabel);
        
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("Search:", GUILayout.Width(50));
//        colorSearchText = EditorGUILayout.TextField(colorSearchText);
//        if (!string.IsNullOrEmpty(colorSearchText) && GUILayout.Button("X", GUILayout.Width(20))) { colorSearchText = ""; GUI.FocusControl(null); }
//        GUILayout.EndHorizontal();

//        if (string.IsNullOrEmpty(colorSearchText))
//        {
//            selectedBlock = (BlockType)EditorGUILayout.EnumPopup("Pen Color:", selectedBlock);
//        }
//        else
//        {
//            DrawSearchableColorList();
//        }

//        GUILayout.Space(5);
//        GUILayout.Label("Fill Actions:", EditorStyles.miniLabel);
//        if (GUILayout.Button($"Fill ALL Grid with {selectedBlock}")) {
//            if(EditorUtility.DisplayDialog("Confirm", $"Fill entire grid?", "Yes", "No")) {
//                Undo.RecordObject(currentLevel, "Fill All");
//                LevelInfo.SmartFill(currentLevel, LevelInfo.FillMode.FillAll, selectedBlock);
//            }
//        }
//        BlockType modeType = LevelInfo.GetModeBlockType(currentLevel);
//        if (GUILayout.Button($"Auto-Repair Empty (Mode: {modeType})")) {
//            Undo.RecordObject(currentLevel, "Auto Fill Mode");
//            LevelInfo.SmartFill(currentLevel, LevelInfo.FillMode.FillEmptyWithMode);
//        }
//        if (GUILayout.Button($"Fill Empty Slots with {selectedBlock}")) {
//            Undo.RecordObject(currentLevel, "Fill Empty Manual");
//            LevelInfo.SmartFill(currentLevel, LevelInfo.FillMode.FillEmptyWithSelection, selectedBlock);
//        }
//        GUILayout.Space(5);
//        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
//        if (GUILayout.Button("Clear Grid (Reset)")) {
//            if(EditorUtility.DisplayDialog("Confirm", "Clear level?", "Yes", "Cancel")) {
//                Undo.RecordObject(currentLevel, "Clear");
//                LevelInfo.SmartFill(currentLevel, LevelInfo.FillMode.FillAll, BlockType.Empty);
//            }
//        }
//        GUI.backgroundColor = Color.white;
//        GUILayout.EndVertical();
//    }

//    void DrawSearchableColorList()
//    {
//        string[] allNames = System.Enum.GetNames(typeof(BlockType));
//        int matchCount = 0;
//        foreach (string name in allNames)
//        {
//            if (name.IndexOf(colorSearchText, System.StringComparison.OrdinalIgnoreCase) >= 0)
//            {
//                matchCount++;
//                BlockType type = (BlockType)System.Enum.Parse(typeof(BlockType), name);
//                GUILayout.BeginHorizontal();
//                Color32 c = paletteCache.ContainsKey(type) ? paletteCache[type] : new Color32(50,50,50,255);
//                Rect r = GUILayoutUtility.GetRect(18, 18, GUILayout.Width(18));
//                EditorGUI.DrawRect(r, c);
//                GUI.backgroundColor = (selectedBlock == type) ? Color.green : Color.white;
//                if (GUILayout.Button(name, EditorStyles.miniButtonLeft)) { selectedBlock = type; GUI.FocusControl(null); }
//                GUI.backgroundColor = Color.white;
//                GUILayout.EndHorizontal();
//            }
//        }
//        if (matchCount == 0) GUILayout.Label("No colors found.", EditorStyles.centeredGreyMiniLabel);
//    }

//    void DrawLevelInfo()
//    {
//        GUILayout.BeginVertical("box");
//        if (currentLevel != null) {
//            var stats = LevelInfo.AnalyzeLevel(currentLevel);
//            GUILayout.Space(5);
//            GUILayout.Label($"Total Pixels: {currentLevel.boxLevelWidth * currentLevel.boxLevelHeight}", EditorStyles.miniLabel);
//            foreach (var stat in stats) {
//                if (stat.count == 0) continue; 
//                GUILayout.BeginHorizontal();
//                Color32 c = paletteCache.ContainsKey(stat.type) ? paletteCache[stat.type] : new Color32(50,50,50,255);
//                Rect r = GUILayoutUtility.GetRect(15, 15, GUILayout.Width(15));
//                EditorGUI.DrawRect(r, c);
//                GUILayout.Label($"{stat.type}", GUILayout.Width(100));
//                GUILayout.Label($"{stat.count}", GUILayout.Width(40));
//                GUILayout.Label($"({stat.percentage:F1}%)", EditorStyles.miniLabel);
//                GUILayout.EndHorizontal();
//            }
//        }
//        GUILayout.EndVertical();
//    }

//    void ProcessImageToGrid()
//    {
//        if (currentLevel == null || sourceImage == null) return;
//        Undo.RecordObject(currentLevel, "Generate Level");
//        RebuildLookupTable();

//        string path = AssetDatabase.GetAssetPath(sourceImage);
//        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
//        if (importer) { importer.isReadable = true; importer.textureCompression = TextureImporterCompression.Uncompressed; importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false; importer.sRGBTexture = true; importer.SaveAndReimport(); }

//        currentLevel.boxLevelWidth = sourceImage.width;
//        currentLevel.boxLevelHeight = sourceImage.height;
//        currentLevel.Initialize();

//        int imgWidth = sourceImage.width;
//        int imgHeight = sourceImage.height;

//        for (int y = 0; y < currentLevel.boxLevelHeight; y++)
//        {
//            for (int x = 0; x < currentLevel.boxLevelWidth; x++)
//            {
//                int texX = x;
//                int texY = imgHeight - 1 - y;
//                Color pixelColor = sourceImage.GetPixel(texX, texY);
//                pixelColor = ApplyColorFilters(pixelColor);

//                int index = y * currentLevel.boxLevelWidth + x;
//                if (pixelColor.a < 0.5f) { currentLevel.boxsList[index].colorType = BlockType.Empty; continue; }

//                if (useStrictMatching) {
//                    if (colorToIdLookup.TryGetValue(pixelColor, out BlockType id)) currentLevel.boxsList[index].colorType = id;
//                    else currentLevel.boxsList[index].colorType = BlockType.Empty;
//                } else {
//                    currentLevel.boxsList[index].colorType = GetClosestBlockType(pixelColor);
//                }
//            }
//        }
//        EditorUtility.SetDirty(currentLevel);
//    }

//    Color ApplyColorFilters(Color c)
//    {
//        float originalAlpha = c.a;
//        float h, s, v;
//        Color.RGBToHSV(c, out h, out s, out v);
//        s = Mathf.Clamp01(s * saturationBoost);
//        if (brightnessBoost != 1.0f) v = Mathf.Clamp01(v * brightnessBoost);
//        Color result = Color.HSVToRGB(h, s, v);
//        result.a = originalAlpha; 
//        return result;
//    }

//    BlockType GetClosestBlockType(Color target)
//    {
//        BlockType closest = BlockType.Empty;
//        double minDist = double.MaxValue;
//        foreach (var pair in paletteCache)
//        {
//            if (pair.Key == BlockType.Empty) continue;
//            Color32 t32 = target; Color32 p32 = pair.Value;
//            long r = t32.r - p32.r; long g = t32.g - p32.g; long b = t32.b - p32.b;
//            double distSq = (2 * r * r) + (4 * g * g) + (3 * b * b);
//            if (distSq < minDist) { minDist = distSq; closest = pair.Key; }
//        }
//        return closest;
//    }

//    void ResizeLevelData(int newW, int newH)
//    {
//        Undo.RecordObject(currentLevel, "Resize Level");
//        List<BoxData> newBlocks = new List<BoxData>();
//        for(int y = 0; y < newH; y++) {
//            for(int x = 0; x < newW; x++) {
//                if (x < currentLevel.boxLevelWidth && y < currentLevel.boxLevelHeight) 
//                {
//                    int oldIndex = y * currentLevel.boxLevelWidth + x;
//                    newBlocks.Add(
//                        new BoxData() { colorType = oldIndex < currentLevel.boxsList.Count ? currentLevel.boxsList[oldIndex].colorType : BlockType.Empty }
//                        );
//                } 
//               else newBlocks.Add(new BoxData());

//            }
//        }
//        currentLevel.boxLevelWidth = newW; currentLevel.boxLevelHeight = newH; currentLevel.boxsList = newBlocks;
//        EditorUtility.SetDirty(currentLevel);
//    }

//    void InsertRow(int yIndex) { Undo.RecordObject(currentLevel, "Add Row"); int idx = yIndex * currentLevel.boxLevelWidth; for(int k=0; k<currentLevel.boxLevelWidth; k++) currentLevel.boxsList.Insert(idx, new BoxData() { colorType =BlockType.Empty }); currentLevel.boxLevelHeight++; EditorUtility.SetDirty(currentLevel); }
//    void RemoveRow(int yIndex) { if (currentLevel.boxLevelHeight <= 1) return; Undo.RecordObject(currentLevel, "Remove Row"); int idx = yIndex * currentLevel.boxLevelWidth; currentLevel.boxsList.RemoveRange(idx, currentLevel.boxLevelWidth); currentLevel.boxLevelHeight--; EditorUtility.SetDirty(currentLevel); }
//    void InsertColumn(int xIndex) { Undo.RecordObject(currentLevel, "Add Col"); for (int y = currentLevel.boxLevelHeight - 1; y >= 0; y--) currentLevel.boxsList.Insert(y * currentLevel.boxLevelWidth + xIndex, new BoxData() { colorType = BlockType.Empty }); currentLevel.boxLevelWidth++; EditorUtility.SetDirty(currentLevel); }
//    void RemoveColumn(int xIndex) { if (currentLevel.boxLevelWidth <= 1) return; Undo.RecordObject(currentLevel, "Remove Col"); for (int y = currentLevel.boxLevelHeight - 1; y >= 0; y--) currentLevel.boxsList.RemoveAt(y * currentLevel.boxLevelWidth + xIndex); currentLevel.boxLevelWidth--; EditorUtility.SetDirty(currentLevel); }

//    void DrawResizeButtons(Rect rectAdd, Rect rectSub, System.Action onAdd, System.Action onSub)
//    {
//        if (GUI.Button(rectAdd, "+")) onAdd.Invoke();
//        if (GUI.Button(rectSub, "-")) onSub.Invoke();
//    }

//    void DrawBorder(Rect r, Color color, float thickness = 1f)
//    {
//        EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, thickness), color); // Top
//        EditorGUI.DrawRect(new Rect(r.x, r.yMax - thickness, r.width, thickness), color); // Bottom
//        EditorGUI.DrawRect(new Rect(r.x, r.y, thickness, r.height), color); // Left
//        EditorGUI.DrawRect(new Rect(r.xMax - thickness, r.y, thickness, r.height), color); // Right
//    }
//}

//public class Color32EqualityComparer : IEqualityComparer<Color32>
//{
//    public bool Equals(Color32 a, Color32 b) => a.r==b.r && a.g==b.g && a.b==b.b && a.a==b.a;
//    public int GetHashCode(Color32 o) => o.r.GetHashCode()^o.g.GetHashCode()^o.b.GetHashCode()^o.a.GetHashCode();
//}
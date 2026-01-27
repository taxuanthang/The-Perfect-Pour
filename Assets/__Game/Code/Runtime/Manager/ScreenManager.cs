using Cysharp.Threading.Tasks;
using Game;
using Kuchen;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public enum ScreenLayer
{
    FullScreen,
    PopUp
}

public enum ScreenName
{
    OutGame_MainMenu,

    //InGame
    InGame_GamePlay,
    InGame_Win,
}

public struct NavigationData
{
    public ScreenName ScreenName;
    public ScreenLayer Layer;
    public object Data;
}
public class ScreenManager : MonoBehaviour
{
    [Header("Out_Game")]
    [SerializeField] public Screen mainMenu_OutGame_Screen;

    [Header("In_Game")]
    [SerializeField] public Screen mainGameplay_InGame_Screen;
    [SerializeField] public Screen win_InGame_Screen;

    //Act as a curtain
    [SerializeField] private int pauseTime = 1;

    private Screen currentScreen;
    private Stack<NavigationData> screenStack = new Stack<NavigationData>();
    private Dictionary<ScreenName, Screen> screenDictionary = new Dictionary<ScreenName, Screen>();


    public void ActiveLoseUI(object data = null)
    {
    }

    private void Start()
    {
        RegisterDictionary();
        SetUp();
    }
    public void RegisterDictionary()
    {
        //OutGame
        screenDictionary.Add(ScreenName.OutGame_MainMenu, mainMenu_OutGame_Screen);


        //InGame
        screenDictionary.Add(ScreenName.InGame_GamePlay, mainGameplay_InGame_Screen);
        screenDictionary.Add(ScreenName.InGame_Win, win_InGame_Screen);


        //Pop-Up

    }

    private void SetUp()
    {
        foreach (var screen in screenDictionary.Values)
        {
            screen.SetUp(this);
        }
    }

    public void NavigateTo(ScreenName screenName, ScreenLayer layer, object data = null)
    {
        // Check nếu Scene đã tồn tại trong Dictionary
        if (!screenDictionary.ContainsKey(screenName))
        {
            Debug.LogError($"Screen '{screenName}' not found!");
            return;
        }
        // Đẩy vào stack
        screenStack.Push(new NavigationData
        {
            ScreenName = screenName,
            Layer = layer,
            Data = data
        });
        // GỌi Stack
        OnStackChanged();


        Screen screen = screenDictionary[screenName];
        screen.OnEnter(data);

    }

    public void ClearAndNavigate(ScreenName screenName, ScreenLayer layer, object data = null)
    {
        screenStack.Clear();
        NavigateTo(screenName, layer, data);
    }


    public void NavigateBack()
    {
        if (screenStack.Count > 1)  //ensure not to pop the last remaining scene
        {
            screenStack.Pop(); // remove the item
        }
        OnStackChanged();
    }

    private void OnStackChanged()
    {
        // lấy những screen được bật
        var visibleScreens = ResolveVisibleScreens();
        // Bật screen
        ApplyVisibility(visibleScreens);

        bool isInputBlocked = false;
        if (screenStack.Count > 0)
        {
            // If the top screen is a PopUp, we block input. 
            // If it's FullScreen we allow input.
            if (screenStack.Peek().Layer == ScreenLayer.PopUp)
            {
                isInputBlocked = true;
            }
        }

    }

    private HashSet<ScreenName> ResolveVisibleScreens()
    {
        //“Lấy screen từ trên xuống
        //Gặp fullscreen thì dừng”
        HashSet<ScreenName> result = new HashSet<ScreenName>();
        bool blocked = false;

        foreach (var nav in screenStack) // top -> bottom
        {
            if (blocked) break;

            result.Add(nav.ScreenName);

            if (nav.Layer == ScreenLayer.FullScreen)
            {
                blocked = true;
            }
        }

        return result;
    }

    private void ApplyVisibility(HashSet<ScreenName> visibleScreens)
    {
        foreach (var pair in screenDictionary)
        {
            bool shouldBeActive = visibleScreens.Contains(pair.Key);
            if (pair.Value == null)
            {
                continue;
            }

            if (pair.Value.gameObject.activeSelf != shouldBeActive)
            {
                pair.Value.gameObject.SetActive(shouldBeActive);
            }
        }
    }

    public async UniTask PlayPauseScreenAsync(CancellationToken token)
    {

        //Wait a moment to ensure the Loading Image covers the screen
        await UniTask.Delay(pauseTime, ignoreTimeScale: true);
        // Wait one more frame to ensure the big Instantiate spike happens   
        await UniTask.NextFrame();
    }
}
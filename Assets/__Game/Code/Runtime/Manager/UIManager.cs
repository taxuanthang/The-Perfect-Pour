using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ScreenManager _screenManager;

    //UPDATE

    // READ
    public void OnGameEnter(PlayerData data)
    {
        _screenManager.NavigateTo(ScreenName.OutGame_MainMenu, ScreenLayer.FullScreen);


        // cập nhật lại hết các Ui dính dáng đến coin
        _screenManager.mainMenu_OutGame_Screen.UpdateUI(data);

        ////// yc screenManager cập nhật UI ở ingamePlayScreen
        _screenManager.mainGameplay_InGame_Screen.UpdateUI(data);

        //_screenManager.inGamePlayScreen.UpdateUI();



    }
    public void OnNewLevel(PlayerData data)
    {
        // yc screen Manager tự cập nhật data
        _screenManager.mainGameplay_InGame_Screen.UpdateUI(data);
        // Chuyển sang scene in game
        _screenManager.NavigateTo(ScreenName.InGame_GamePlay, ScreenLayer.FullScreen);
    }


    public void OnLevelWin(PlayerData data)
    {
        _screenManager.NavigateTo(ScreenName.InGame_Win, ScreenLayer.FullScreen);
        //WE DONT UPDATE COIN UI WHEN WIN--> only increase the money --. we update winning coin UI when OnGameEnter or when move to the Menu when win LV
        //_screenManager.outgameMainMenuScreen.UpdateUI(data);
        _screenManager.win_InGame_Screen.UpdateUI(data);
    }

    public void OnLevelWin(WinState data)
    {
        //WE DONT UPDATE COIN UI WHEN WIN--> only increase the money --. we update winning coin UI when OnGameEnter or when move to the Menu when win LV
        //_screenManager.outgameMainMenuScreen.UpdateUI(data);
        _screenManager.win_InGame_Screen.UpdateUI(data);
    }

    //public void OnLevelLose(PlayerData data, int retryCost)
    //{
    //    _screenManager.loseScreenConfirm.UpdateUI(data); //Udpate cin data  
    //    Debug.Log("UPDATE UI FOR LOSE SCRENNNNNN2");
    //    var loseData = new LoseConfirmData  //simple data package to carry the retryCost and main data
    //    {
    //        _currentCoins = data.money,
    //        _retryCost = retryCost
    //    };
    //    _screenManager.ActiveLoseUI(loseData);
    //    _screenManager.loseScreenConfirm.UpdateUI(loseData);
    //    _screenManager.outgameMainMenuScreen.UpdateUI(data);
    //    _screenManager.levelScreen.UpdateUI(data);
    //}

    public async UniTask PlayLoadingScreen()
    {
        await _screenManager.PlayPauseScreenAsync(this.GetCancellationTokenOnDestroy());
    }


}

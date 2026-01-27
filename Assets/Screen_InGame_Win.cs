using Game;
using Kuchen;
using UnityEngine;
using UnityEngine.UI;

public class Screen_InGame_Win : Screen
{
    [SerializeField] Button _nextLevelButton;
    [SerializeField] Coin _coinUI;
    [SerializeField] Button _x2RewardButton;
    [SerializeField] Star _starUI;

    public override void OnExit()
    {

    }

    public override void UpdateUI<T>(T Data)
    {
        if (Data is PlayerData playerData)
        {
            string moneyValue = playerData.money.ToString();
            _coinUI.UpdateCoin(moneyValue);
            //_levelText.text = $"Level {playerData.levelConfigIndex.ToString()}";
        }

        if(Data is WinState winState)
        {
            _starUI.ShowStar(winState);
        }

    }

    public override void AssignButtons()
    {
        _nextLevelButton.onClick.AddListener(NextLevel);
        //_coinBtn.onClick.AddListener(OpenShop);

    }

    private void NextLevel()
    {
        _screenManager.NavigateTo(ScreenName.InGame_GamePlay, ScreenLayer.FullScreen);
        this.Publish(GameEvent.NEW_GAME);
    }
}

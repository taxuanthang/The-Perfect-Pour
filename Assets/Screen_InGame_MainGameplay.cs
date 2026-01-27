using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen_InGame_MainGameplay : Screen
{
    //TOp
    [SerializeField] Button _pauseButton;
    [SerializeField] Button _replayButton;
    [SerializeField] Coin _coinUI;
    [SerializeField] Level _levelUI;
    [SerializeField] TextMeshProUGUI _levelText;


    public override void OnExit()
    {

    }

    public override void AssignButtons()
    {
        //_settingButton.onClick.AddListener(OpenSetting);
        //_coinBtn.onClick.AddListener(OpenShop);

    }

    //public void OpenSetting()
    //{
    //    _screenManager.NavigateTo(ScreenName.InGame_Settings, ScreenLayer.PopUp);
    //}

    //public void OpenShop()
    //{
    //    _screenManager.NavigateTo(ScreenName.InGame_Shop, ScreenLayer.PopUp);
    //}

    public override void UpdateUI<T>(T Data)
    {
        if (Data is PlayerData playerData)
        {
            string moneyValue = playerData.money.ToString();
            _coinUI.UpdateCoin(moneyValue);
            _levelUI.UpdateLevel(playerData.levelConfigIndex.ToString());
            //_levelText.text = $"Level {playerData.levelConfigIndex.ToString()}";
        }

    }
}

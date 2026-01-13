using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen_InGame_MainGameplay : Screen
{
    //TOp
    [SerializeField] Button _settingButton;

    [SerializeField] Button _coinBtn;
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] TextMeshProUGUI _numberOfPigsOnBelt;



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
            //string content = playerData.money.ToString();
            //_coinText.text = content;
            //_levelText.text = $"Level {playerData.levelConfigIndex.ToString()}";
        }

    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen_OutGame_MainMenu : Screen
{
    [Header("Top")]
    [SerializeField] Button setingButon;

    [Header("Pages")]

    [Header("Bottom")]
    [SerializeField] Button levelButton;
    [SerializeField] Button shopButton;

    [SerializeField] Button coinBtn;
    public TextMeshProUGUI coinText;



    public override void AssignButtons()
    {
        ////Top
        //setingButon.onClick.AddListener(OnClickSettings);
        //coinBtn.onClick.AddListener(OnShopButtonClick);
        ////Bottom
        //levelButton.onClick.AddListener(OnLevelButtonClick);
        //shopButton.onClick.AddListener(OnShopButtonClick);

    }

    public override void OnEnter(object data)
    {
        base.OnEnter(data);
        //_screenManager.NavigateTo(ScreenName.OutGame_Level, ScreenLayer.PopUp);
    }

    public override void OnExit()
    {

    }

    public override void UpdateUI<T>(T data)
    {
        if (data is PlayerData playerData)
        {
            //cập nhật máu

            //cập nhật vàng--Ok bro
            if (data is PlayerData playerData2)
            {
                string content = playerData.money.ToString();
                coinText.text = content;

            }

        }


    }




    //public void OnClickSettings()
    //{
    //    _screenManager.NavigateTo(ScreenName.OutGame_Settings, ScreenLayer.PopUp);
    //}

    //public void OnLevelButtonClick()
    //{
    //    _screenManager.NavigateBack();
    //    _screenManager.NavigateTo(ScreenName.OutGame_Level, ScreenLayer.PopUp);
    //}

    //public void OnShopButtonClick()
    //{
    //    _screenManager.NavigateBack();
    //    _screenManager.NavigateTo(ScreenName.OutGame_Shop, ScreenLayer.PopUp);
    //}

}

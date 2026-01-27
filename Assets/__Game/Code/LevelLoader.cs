using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    LevelData data;
    
    public LevelData GetLevelData()
    {
        return data; 
    }

    public void LoadLevelConfigByIndex(int levelDataIndex)  //function to load the levelDataIndex from the JsonReader and put it in levelLoader with Corressponding LevelData
    {
        if (levelDataIndex >= 0) //valid index number 
        {
            string levelDataPath = "Level " + levelDataIndex;
            LevelData newLevelData = Resources.Load<LevelData>(levelDataPath);

            SetLevelData(newLevelData);
        }
        else
        {
            Debug.Log("Wrong data from Json File Data!!!");
        }

        //GamePlayPresenter.instance.onLoadLevelDataComplete(levelToLoad);

    }



    public LevelData GetCurrentLevelData() => data;

    public void SetLevelData(LevelData newLevelData)
    {
        data = newLevelData;
    }
}


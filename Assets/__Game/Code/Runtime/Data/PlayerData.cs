using System;
using UnityEngine;


    [System.Serializable]
    public class PlayerData
    {
        [Header("LevelDataIndex")]
        public int levelConfigIndex = 1;

        [Header("LastPlay Time")]
        public string lastPlayTime = DateTime.Now.ToString();

        [Header("Money")]
        public int money = 0;

        [Header("Life")]
        public int life = 0;

        //[Header("HelperSkill")]


        public PlayerData(int levelConfigIndex = 0, string lastPlayTime = "0", int money = 1900, int life = 5)
        {
            this.levelConfigIndex = levelConfigIndex;
            this.lastPlayTime = lastPlayTime;
            this.money = money;


            this.life = life;
        }

        public PlayerData()
        {
            //Default Data
            this.levelConfigIndex = 1;
            this.lastPlayTime = DateTime.Now.ToString();
            this.money = 1900;

            this.life = 5;
        }
    }


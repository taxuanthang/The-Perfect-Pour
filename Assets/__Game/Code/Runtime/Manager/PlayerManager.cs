using Game;
using Kuchen;
using System;
using UnityEngine;

    public class PlayerManager : MonoBehaviour
    {
        string playerName;
        [SerializeField] PlayerData currentPlayerData;
        private int _currentLvLostCount = 0;

        // 

        //Life
        private float healthTimer;
        private const int MAX_LIFE = 5;
        private const float HEALTH_RECOVER_TIME = 1200f; // 20 phút

        private float uiUpdateTimer;


        public void SetUp()
        {
            //GamePlayPresenter.instance.onWin += OnWin;
            //GamePlayPresenter.instance.onLose += OnLose;

        }

        public void OnDisable()
        {
        }

        public void Tick()
        {
            //HealthTimerCountDown();

        }
        public PlayerData GetPlayerData()
        {
            return currentPlayerData;

        }

        public void AdjustCoin(int value)
        {
            currentPlayerData.money += value;
        }


        // Data Saving and Loading Methods
        public void SaveGameDataFromCurrentCharacterData(PlayerData currentPlayerData)
        {
            this.currentPlayerData.lastPlayTime = DateTime.Now.ToString();
            currentPlayerData = this.currentPlayerData;
        }

        public void LoadGameDataFromCurrentCharacterData(PlayerData currentPlayerData)
        {
            this.currentPlayerData = currentPlayerData;

        }

        public int GetLevelIndex() => currentPlayerData.levelConfigIndex;

        public void OnWin(WinState winState)
        {
            currentPlayerData.life = Mathf.Min(currentPlayerData.life + 1, MAX_LIFE); //tăng máu khi win
            currentPlayerData.levelConfigIndex++;

            switch (winState)
            {
                case WinState.None:
                    break;
                case WinState.Green:
                    AdjustCoin(40);  // money always incresase 40 when win (for all level)--> magic number 
                    break;
                case WinState.Yellow:
                    AdjustCoin(20);  // money always incresase 40 when win (for all level)--> magic number 
                    break;
                case WinState.Red:
                    AdjustCoin(0);  // money always incresase 40 when win (for all level)--> magic number 
                    break;

            }

            _currentLvLostCount = 0;
        }

        public void OnLose()
        {
            _currentLvLostCount++;
        }


        public int GetCurrentReplayCost()
        {
            // Use Mathf.Max to ensure we dont get negative numbers if logic slips
            int count = Mathf.Max(1, _currentLvLostCount);
            return 900 + (count - 1) * 1000;
        }

        // Call if the player quits to main menu instead of retrying
        public void ResetLoseStreak()
        {
            _currentLvLostCount = 0;
        }


        //public void HealthTimerCountDown()
        //{
        //    // Nếu đã full máu
        //    if (currentPlayerData.life >= MAX_LIFE)
        //    {
        //        healthTimer = HEALTH_RECOVER_TIME;
        //        uiUpdateTimer = 0f;

        //        PublishHealthUI("MAX");
        //        return;
        //    }

        //    // Đếm ngược hồi máu
        //    healthTimer -= Time.deltaTime;
        //    uiUpdateTimer += Time.deltaTime;

        //    // Update UI mỗi 1 giây
        //    if (uiUpdateTimer >= 1f)
        //    {
        //        uiUpdateTimer = 0f;

        //        string formattedTime =
        //            TimeSpan.FromSeconds(Mathf.Max(healthTimer, 0))
        //            .ToString(@"mm\:ss");

        //        PublishHealthUI(formattedTime);
        //    }


        //    // Hết thời gian → cộng máu
        //    if (healthTimer <= 0f)
        //    {
        //        currentPlayerData.life++;
        //        healthTimer = HEALTH_RECOVER_TIME;

        //        PublishHealthUI("20:00");
        //    }
        //}

        //private void PublishHealthUI(string timeString)
        //{
        //    this.Publish(GameEvent.OnLifeChange,
        //        new HealthUIData
        //        {
        //            timeString = timeString,
        //            currentHealth = currentPlayerData.life
        //        });
        //}

        public int UpdateFromOffline()
        {

            // Update currentHealth
            DateTime lastPlay = DateTime.Parse(currentPlayerData.lastPlayTime);
            TimeSpan timeGap = DateTime.Now - lastPlay;


            int recoveredLife = (int)(timeGap.TotalSeconds / HEALTH_RECOVER_TIME);
            int result = Mathf.Min(currentPlayerData.life + recoveredLife, MAX_LIFE);

            currentPlayerData.life = result;

            //Update healthTimer
            healthTimer = (recoveredLife + 1) * 1200f - (float)timeGap.TotalSeconds;

            string formattedTime =
                TimeSpan.FromSeconds(Mathf.Max(healthTimer, 0))
                .ToString(@"mm\:ss");

            //Update Health UI
            //PublishHealthUI(formattedTime);

            return result;
        }

        public void MinusHealth(int value)
        {
            currentPlayerData.life -= value;
        }

        //public bool CheckHealthEmpty()
        //{
        //    if (currentPlayerData.life <= 0)
        //    {
        //        this.Publish(GameEvent.OnOutOfHealth);
        //        return true;
        //    }

        //    else
        //    {
        //        return false;
        //    }
        //}

    }

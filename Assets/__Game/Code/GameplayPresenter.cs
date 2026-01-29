using Cysharp.Threading.Tasks;
using Game;
using Kuchen;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;



namespace Game
{
    public class GameplayPresenter : MonoBehaviour
    {
        [SerializeField]
        private Bottle _bottle;

        [SerializeField]
        private Faucet _faucet;

        [Header("Managers")]
        [SerializeField]
        private LevelLoader _levelLoader;
        [SerializeField]
        private UIManager _uiManager;
        [SerializeField]
        private PlayerManager _playerManager;
        [SerializeField]
        MainManager _mainManager;

        CancellationTokenSource waterCTS;

        void OnEnable()
        {
            waterCTS = new CancellationTokenSource();
        }

        void OnDisable()
        {
            waterCTS.Cancel();
            waterCTS.Dispose();
        }

        public void SetUp()
        {
            this.Subscribe(GameEvent.ENTER_GAME, OnGameEnter);
            this.Subscribe(GameEvent.NEW_GAME, OnNewGame);




            //Input Event
            this.Subscribe(GameEvent.OnHolded, OnHold);
            this.Subscribe(GameEvent.OnCliked, OnClick);
            this.Subscribe(GameEvent.OnPointerDown, OnPointerDown);
            this.Subscribe(GameEvent.OnPointerUp, OnPointerUp);


        }


        void OnGameEnter()
        {
            //StopPlaying();

            ////Load data vào playerData
            //PlayerData data = _saveGameManager.LoadGame();
            //_playerManager.LoadGameDataFromCurrentCharacterData(data);

            //
            var data = _playerManager.GetPlayerData();
            // cập nhật data vừa load vào UI
            _uiManager.OnGameEnter(data);

            //int currentHealth = _playerManager.UpdateFromOffline();

            //
            var levelData = _levelLoader.GetLevelData();

            Sprite paddedImage = SpriteScaler.ScaleAndPadSprite(
                levelData.bottle,
                750,     // khung mới
                750,
                AnchorMode.Bottom   // ví dụ: đặt ảnh ở đáy
            );


            Sprite paddedImage2 = SpriteScaler.ScaleAndPadSprite(
                levelData.layer,
                750,     // khung mới
                750,
                AnchorMode.Bottom   // ví dụ: đặt ảnh ở đáy
            );

            var bottleData = new BottleData()
            {
                bottle = paddedImage,
                layer = paddedImage2,
                redSize1 = levelData.redSize1,
                yellowSize1 = levelData.yellowSize1,
                greenSize = levelData.greenSize,
                yellowSize2 = levelData.yellowSize2,
                redSize2 = levelData.redSize2,

                waterType = levelData.waterType,

                goal = levelData.goal,

                listIncreasing = levelData.listIncreasing,
            };

            _bottle.GenerateLevel(bottleData);
            //


        }

        async void OnNewGame()
        {
            //if (_playerManager.CheckHealthEmpty())
            //{
            //    return;
            //}

            //// trừ máu người chơi
            //_playerManager.MinusHealth(1);

            //Reset Level
            print("New Game Started");
            PlayerData data = _playerManager.GetPlayerData();
            //loadLevelData hiện tại 
            _levelLoader.LoadLevelConfigByIndex(data.levelConfigIndex);

            GenerateLevel();

            await _uiManager.PlayLoadingScreen();
            //Cập nhật UI
            _uiManager.OnNewLevel(data);


            ResumePlaying();

        }

        void StopPlaying()
        {
            _mainManager.Pause();



        }

        void ResumePlaying()
        {
            _mainManager.Resume();

        }



        public void GenerateLevel()
        {
            var levelData = _levelLoader.GetLevelData();

            Sprite paddedImage = SpriteScaler.ScaleAndPadSprite(
                levelData.bottle,
                750,     // khung mới
                750,
                AnchorMode.Bottom   // ví dụ: đặt ảnh ở đáy
            );


            Sprite paddedImage2 = SpriteScaler.ScaleAndPadSprite(
                levelData.layer,
                750,     // khung mới
                750,
                AnchorMode.Bottom   // ví dụ: đặt ảnh ở đáy
            );

            var bottleData = new BottleData()
            {
                bottle = paddedImage,
                layer = paddedImage2,
                redSize1 = levelData.redSize1,
                yellowSize1 = levelData.yellowSize1,
                greenSize = levelData.greenSize,
                yellowSize2 = levelData.yellowSize2,
                redSize2 = levelData.redSize2,

                waterType = levelData.waterType,

                goal = levelData.goal,

                listIncreasing = levelData.listIncreasing,
            };

            _faucet.SetUp(levelData.waterType,levelData.faucetType);

            _bottle.GenerateLevel(bottleData);
        }



        bool triggerOnce = false;

        // Input
        public void OnHold()
        {
        }
        public void OnClick()
        {
        }
        public void OnPointerDown()
        {
            if (_mainManager.isPaused)
            {
                return;
            }
            if(!triggerOnce)
            {
                this.Publish(GameEvent.NEW_GAME);
            }

            _bottle.SetPour(true);
            _faucet.SetPour(true);
        }
        public async void OnPointerUp()
        {

            if (_mainManager.isPaused)
            {
                return;
            }

            StopPlaying();
            triggerOnce = true;
            _faucet.SetPour(false);
            _bottle.SetPour(false);
            _bottle.SetGoalActive(true);

            await DelayWater();

            WinState winState = _bottle.GetWinState();
            Debug.Log(winState);
            switch (winState) 
            { 
                case WinState.None:
                    break;
                case WinState.Green:
                    break;
                case WinState.Yellow: 
                    break;
                case WinState.Red: 
                    break;

            }
            // cộng data cho player
            _playerManager.OnWin(winState);

            // stop game play

            var data = _playerManager.GetPlayerData();
            // play win/lose screen
            //Update UI theo playerData
            _uiManager.OnLevelWin(data);
            //Update UI theo winstate
            _uiManager.OnLevelWin(winState);


        }



        public async Task DelayWater()
        {
            try
            {
                await Task.Delay(1000, cancellationToken: waterCTS.Token);

                int repeatTime = UnityEngine.Random.Range(3, 5);

                for (int i = 0; i < repeatTime; i++)
                {
                    if(i == repeatTime-1)
                    {
                        await CreateWater();
                        await Task.Delay(1500, cancellationToken: waterCTS.Token);
                    }
                    else
                    {
                        CreateWater(); 
                        await Task.Delay(400, cancellationToken: waterCTS.Token);
                    }

                }

                // Start lava decrease after all droplets are done
                _bottle.StartLavaDecrease();
            }
            catch (OperationCanceledException) { }
        }

        public async Task CreateWater()
        {
            _faucet.CreateDelayWater();

            await Task.Delay(1500, cancellationToken: waterCTS.Token);

            _bottle.CreateDelayWater();
        }
    }
}


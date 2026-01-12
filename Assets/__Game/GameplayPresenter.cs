using Game;
using Kuchen;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public void SetUp()
        {
            //Input Event
            this.Subscribe(GameEvent.OnHolded, OnHold);
            this.Subscribe(GameEvent.OnCliked, OnClick);
            this.Subscribe(GameEvent.OnPointerDown, OnPointerDown);
            this.Subscribe(GameEvent.OnPointerUp, OnPointerUp);

            GenerateLevel();
        }

        public void GenerateLevel()
        {
            var levelData = _levelLoader.GetLevelData();
            var bottleData= new BottleData()
            {
                bottle = levelData.bottle,
                redSize1 = levelData.redSize1,
                yellowSize1 = levelData.yellowSize1,
                greenSize = levelData.greenSize,
                yellowSize2 = levelData.yellowSize2,
                redSize2 = levelData.redSize2,

                goal = levelData.goal,

                listIncreasing = levelData.listIncreasing,
            };
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
            if(!triggerOnce)
            {
                _bottle.SetPour(true);
                _faucet.SetPour(true);
            }    
        }
        public void OnPointerUp()
        {
            if(triggerOnce)
            {
                return;
            }
            triggerOnce = true;
            _faucet.SetPour(false);
            _bottle.SetPour(false);

            DelayWater();

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
        }

        public async void DelayWater()
        {
            await Task.Delay(1000);
            int repeatTime = Random.Range(3, 5);
            for (int i = 0; i < repeatTime; i++)
            {
                CreateWater();
                await Task.Delay(400);
            }
        }

        public async void CreateWater()
        {
            _faucet.CreateDelayWater();
            await Task.Delay(2000);
            _bottle.CreateDelayWater();
        }
    }
}


using Game;
using Kuchen;
using System.Collections;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    GameplayPresenter _presenter;

    [SerializeField]
    public bool isPaused = false;


    public void Update()
    {
        
    }

    IEnumerator Start()
    {
        _presenter.SetUp();
        yield return null; // đợi bootstrap
        this.Publish(GameEvent.ENTER_GAME);
    }

    public void Pause()
    {
        if (isPaused)
        {
            Debug.Log("Already Paused");

        }


        isPaused = true;
        //Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        if (!isPaused)
        {
            Debug.Log("Already resume");

        }

        isPaused = false;
        // Time.timeScale = 1f;
        AudioListener.pause = false;
    }

}

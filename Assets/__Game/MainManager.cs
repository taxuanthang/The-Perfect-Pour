using Game;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    GameplayPresenter _presenter;

    [SerializeField]
    bool _isPlaying = false;

    public void Start()
    {
        _presenter.SetUp();
    }

    public void Update()
    {
        
    }

}

using Game;
using UnityEngine;

public class Star : MonoBehaviour
{ 
    [SerializeField] GameObject[] _stars = new GameObject[3];
    public void ShowStar(WinState winState)
    {
        switch (winState)
        {
            case WinState.None:
                break;
            case WinState.Green:
                foreach (var star in _stars)
                {
                    star.SetActive(true);
                }
                break;
            case WinState.Yellow:
                _stars[0].SetActive(true);
                _stars[1].SetActive(false);
                _stars[2].SetActive(true);
                break;
            case WinState.Red:
                foreach (var star in _stars)
                {
                    star.SetActive(false);
                }
                break;

        }
    }
}

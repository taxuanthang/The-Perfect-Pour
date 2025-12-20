using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public Sprite bottle;
    public float redSize1 = 15f;
    public float yellowSize1 = 20f;
    public float greenSize = 30f;
    public float yellowSize2 = 20f;
    public float redSize2 = 15f;

    public float goal = 80f;

    public List<SpeedIncreasing> listIncreasing;

}

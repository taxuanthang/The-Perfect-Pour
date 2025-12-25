using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public Sprite bottle;                                                               // sprite của bottle
    public float redSize1 = 1f;                                                         // 
    public float yellowSize1 = 0.8f;
    public float greenSize = 0.7f;
    public float yellowSize2 = 0.5f;                                                    // 0.3-0.5f là màu vàng 2
    public float redSize2 = 0.3f;                                                       // 0-0.3f là màu đỏ 2

    public float goal = 0.65f;                                                          // 0.65f là vạch

    public List<SpeedIncreasing> listIncreasing;                                        // Speed: increase, Size: 0.3f là đến mức 0.3 thì tăng 2x, 3x

}

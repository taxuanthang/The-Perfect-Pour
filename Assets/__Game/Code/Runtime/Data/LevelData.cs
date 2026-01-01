using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public Sprite bottle; // sprite của bottle

    [Header("Scene Bottle Target (Per Level)")]
    public GameObject bottleTarget; // Kéo Bottle từ Hierarchy vào đây cho từng level riêng

    public float redSize1 = 1f;
    public float yellowSize1 = 0.8f;
    public float greenSize = 0.7f;
    public float yellowSize2 = 0.5f;
    public float redSize2 = 0.3f;
    public float goal = 0.65f;

    public List<SpeedIncreasing> listIncreasing;
}
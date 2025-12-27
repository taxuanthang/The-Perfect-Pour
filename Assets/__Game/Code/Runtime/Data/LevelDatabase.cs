using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Data/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelData> levels = new();
}

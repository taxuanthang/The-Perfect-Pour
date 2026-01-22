using System;
using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _levelText;
    internal void UpdateLevel(string levelConfigIndex)
    {
        _levelText.text = "Level "+ levelConfigIndex;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class WaterEffect : MonoBehaviour
{
    ParticleSystem particleSystem;

    public void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        }
    }
}

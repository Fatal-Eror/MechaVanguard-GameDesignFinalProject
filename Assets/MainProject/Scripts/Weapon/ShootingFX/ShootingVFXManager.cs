using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingVFXManager : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    void Start()
    {
        if(particleSystems != null)
        {
            foreach (var ps in particleSystems)
            {
                ps.Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

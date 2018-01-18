using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEnd : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, particleSystem.duration + 1.0f);
    }

}

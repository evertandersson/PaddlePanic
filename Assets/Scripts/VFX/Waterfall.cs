using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour
{
    ParticleSystem particleSystemComp = null;
    void Start()
    {
        particleSystemComp = GetComponent<ParticleSystem>();

        if (particleSystemComp) {
            var main = particleSystemComp.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.black, Color.white);
            var mode = main.startColor.mode;
            mode = ParticleSystemGradientMode.TwoGradients;

            //particleSystemComp.main = main;

            Debug.Log(particleSystemComp.main.startColor.mode);

        }
    }
}

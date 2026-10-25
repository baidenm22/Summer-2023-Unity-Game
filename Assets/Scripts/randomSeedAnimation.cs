using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomSeedAnimation : MonoBehaviour
{
    [SerializeField] Animator objectAnimator;
    [SerializeField] int maxSeed; //inclusive

    private void Awake()
    {
        int seed = (int) Random.Range(0, maxSeed + 0.99f);
        objectAnimator.SetInteger("seed", seed);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

public class AudioMangement : MonoBehaviour
{
    [SerializeField] AudioSource[] hurt;

    void OnEnable()
    {
        CharcterController.onHurt += hurtSound;
    }
    private void OnDisable()
    {
        CharcterController.onHurt -= hurtSound;
    }

    public void hurtSound()
    {
        int index = Random.Range(0, hurt.Length - 1);
        hurt[index].Play();
    }


}

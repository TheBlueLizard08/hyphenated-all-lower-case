using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EngineRevving : MonoBehaviour
{
    public float idleVolume, maxVolume;
    public float idlePitch, maxPitch;
    public float upSpeedMod, downSpeedMod;
    float curPos = 0.0f;

    AudioSource audioSource;
    bool canRev = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();        
        audioSource.pitch = idlePitch;
        audioSource.volume = 0.0f;
    }

    private void Update()
    {
        HandleEngineRev();
    }

    void HandleEngineRev()
    {
        if (!canRev) { return; }
        //Using inline checks to make my code look incomprihensible? Never
        curPos += Time.deltaTime * (Input.GetKey(KeyCode.W) ? upSpeedMod : -downSpeedMod);
        
        curPos = Mathf.Clamp(curPos, 0.0f, 1.0f);
        audioSource.volume = Mathf.Lerp(idleVolume, maxVolume, curPos);
        audioSource.pitch = Mathf.Lerp(idlePitch, maxPitch, curPos);
    }

    public void FadeInEngine()
    {
        StartCoroutine(FadeInEngineC());
    }

    IEnumerator FadeInEngineC()
    {
        audioSource.Play();
        float t = 0.0f;
        while(t<=1.0f)
        {
            audioSource.volume = Mathf.Lerp(0.0f, idleVolume, t);
            t += Time.deltaTime;
            yield return null;
        }

        canRev = true;
    }
}

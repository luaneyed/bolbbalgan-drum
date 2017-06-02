using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class SoundDrum : Drum {
    private AudioSource audio;

    void Start()
    {
        base.Start();
        audio = GetComponent<AudioSource>();
    }

    abstract protected float getIntensity();

    private void play(AudioSource audio, float intensity)
    {
        audio.volume = intensity;
        audio.Play();
    }

    virtual protected AudioSource getAudio()
    {
        return audio;
    }

    override protected void onHit()
    {
        play(getAudio(), getIntensity());
    }
}

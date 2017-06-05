using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class SoundDrum : Drum {
    private static Color normalColor = Color.white;
    private static Color excitedColor = Color.red;
    private static float hitEffectDuration = 0.2f;

    private AudioSource audio;
    abstract protected string drumName { get; }
    private float lastHitTime = 0f;

    void Start()
    {
        base.Start();

        int audioIdx = PlayerPrefs.GetInt("SoundIdx");
        audio = GetComponents<AudioSource>()[audioIdx];
    }

    protected void Update()
    {
        base.Update();

        if (state.MainStatus == State.Status.Playing)
        {
            float curTime = Time.time;
            if (curTime >= lastHitTime && curTime <= lastHitTime + hitEffectDuration)
            {
                if (curTime <= lastHitTime + (hitEffectDuration / 2))
                {
                    float t = (curTime - lastHitTime) / (hitEffectDuration / 2);
                    setColor(Color.Lerp(normalColor, excitedColor, t));
                }
                else
                {
                    float t = (curTime - lastHitTime - (hitEffectDuration / 2)) / (hitEffectDuration / 2);
                    setColor(Color.Lerp(excitedColor, normalColor, t));
                }
            }
            else
            {
                setColor(normalColor);
            }
        }
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
        lastHitTime = Time.time;
    }

    private void setColor(Color color)
    {
        GameObject.Find(drumName).GetComponent<MeshRenderer>().material.color = color;
    }
}

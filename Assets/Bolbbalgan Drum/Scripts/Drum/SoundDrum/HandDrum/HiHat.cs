using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class HiHat : HandDrum
{
    override protected string drumName { get { return "Hi-hat"; } }
    override protected float radius { get { return 2; } }

    public AudioSource openAudio;
    public AudioSource closeAudio;
    private bool wasHiHatOpened;
    void Start()
    {
        int audioIdx = PlayerPrefs.GetInt("SoundIdx");

        openAudio = GetComponents<AudioSource>()[2*audioIdx + 1];
        closeAudio = GetComponents<AudioSource>()[2*audioIdx];

        base.Start();
    }

    protected void Update()
    {
        base.Update();
        
        if (wasHiHatOpened && !motion.isHiHatOpened)
        {
            openAudio.Stop();
        }
        wasHiHatOpened = motion.isHiHatOpened;
    }

    override protected AudioSource getAudio()
    {
        if (motion.isHiHatOpened)
        {
            return openAudio;
        }
        else
        {
            return closeAudio;
        }
    }
}

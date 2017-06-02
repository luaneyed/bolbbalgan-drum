using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class HiHat : HandDrum
{
    public AudioSource openAudio;
    public AudioSource closeAudio;
    void Start()
    {
        openAudio = GetComponents<AudioSource>()[1];
        closeAudio = GetComponents<AudioSource>()[0];

        base.Start();
    }

    override protected float radius { get { return 2; } }

    override protected AudioSource getAudio()
    {
        //Debug.Log("play" + motion.isHiHatOpened);
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

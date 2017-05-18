using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour {

    [SerializeField]
    AudioMixer masterMixer;
    [SerializeField]
    Toggle muteButton;
    private float origVol;
    private bool muted = false;

    public void SetSfxLevel(float sfxLevel)
    {
        masterMixer.SetFloat("sfxVol", sfxLevel);
    }

    public void SetMusicLevel(float musicLevel)
    {
        masterMixer.SetFloat("musicVol", musicLevel);
    }

    public void SetMasterLevel(float masterLevel)
    {
        masterMixer.SetFloat("masterVol", masterLevel);
    }

    public void Mute()
    {
        if (!muted)
        {
            masterMixer.GetFloat("masterVol", out origVol);
            masterMixer.SetFloat("masterVol", -80f);
            muted = !muted;
        }
        else
        {
            masterMixer.SetFloat("masterVol", origVol);
            muted = !muted;
        }
    }
}

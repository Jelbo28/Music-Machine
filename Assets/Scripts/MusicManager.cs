using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private Object[] localFiles;
    [SerializeField]
    List<AudioClip> localMusic = new List<AudioClip>();
    [SerializeField]
    string folderPath = "Songs";
    [SerializeField]
    private Text songListText;
	// Use this for initialization
	void Start () {
        //fileCount = Resources.LoadAll(folderPath).Length;
        localFiles = Resources.LoadAll(folderPath);
        foreach (Object file in localFiles)
        {
            if (file.GetType() == typeof(AudioClip))
            {
                localMusic.Add((AudioClip)file);
                songListText.text = songListText.text + "\n" + file.name;
            }
        }
        //for (int i = 0; i < fileCount; i++)
        //{
        //    localMusic.Add(Resources.Load(folderPath))
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

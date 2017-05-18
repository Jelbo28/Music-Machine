using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using UnityEngine.UI;
using System.Linq;

public class MusicPlayer : MonoBehaviour
{
    public enum SeekDirection { Forward, Backward }

    [SerializeField] private Text songListText;
    public AudioSource source;
    public List<AudioClip> clips = new List<AudioClip>();

    [SerializeField]
    [HideInInspector]
    private int currentIndex = 0;
    private List<FileInfo> soundFiles = new List<FileInfo>();
    private List<string> validExtensions = new List<string> { ".ogg", ".wav", ".mp3" }; // Don't forget the "." i.e. "ogg" won't work - cause Path.GetExtension(filePath) will return .ext, not just ext.
    private string absolutePath = "./Songs"; // relative path to where the app is running - change this to "./music" in your case
    private bool paused = true;
    private bool start = true;
    private bool shuffleMode = true;
    private string origSongListText;
    private int prevIndex;
    private int backwards = 0;
    [SerializeField] private Image[] buttonUI;
    [SerializeField] private Sprite[] playButtonSprites;
    [SerializeField] private Text shuffleText;
    [SerializeField] private Text currSongText;
    [SerializeField] private List<int> trackList; 
    void Start()
    {
        //being able to test in unity
        buttonUI[0].sprite = playButtonSprites[0];
        if (Application.isEditor) absolutePath = "Assets/Songs/";

        if (source == null) source = gameObject.AddComponent<AudioSource>();
        origSongListText = songListText.text;
        ReloadSounds();
    }

    void Update()
    {
        if (!source.isPlaying && !paused && !start)
        {
            if (shuffleMode)
                RandomSong();
            else
            {
                NextSong();
            }
        }
    }

    //void OnGUI()
    //{
    //    if (GUILayout.Button("Previous"))
    //    {
    //        Seek(SeekDirection.Backward);
    //        PlayCurrent();
    //    }
    //    if (GUILayout.Button("Play current"))
    //    {
    //        PlayCurrent();
    //    }
    //    if (GUILayout.Button("Next"))
    //    {
    //        Seek(SeekDirection.Forward);
    //        PlayCurrent();
    //    }
    //    if (GUILayout.Button("Reload"))
    //    {
    //        ReloadSounds();
    //    }
    //}

    public void Play()
    {
        if (!start)
        {
            if (paused)
                Resume();
            else
            {
                Pause();
            }
        }
        else
        {
            PlayCurrent();
            start = false;
        }
    }

    public void PreviousSong()
    {
        Seek(SeekDirection.Backward);
        PlayCurrent();
    }

    public void NextSong()
    {
        backwards = 0;
        if (!shuffleMode)
        {
            Seek(SeekDirection.Forward);
            PlayCurrent();
        }
        else
        {
            RandomSong();
        }
    }

    public void ShuffleMode()
    {
        shuffleMode = !shuffleMode;
        shuffleText.text = shuffleMode ? "\"On\"" : "\"Off\"";
    }

    public void ReloadSongs()
    {
        ReloadSounds();
    }

    void Seek(SeekDirection dir)
    {
        prevIndex = currentIndex;
        if (dir == SeekDirection.Forward)
            currentIndex = (currentIndex + 1) % clips.Count;
        else if (dir == SeekDirection.Backward)
        {
            backwards++;
            currentIndex = trackList[(trackList.Count -1) - backwards];
            //if (currentIndex < 0) currentIndex = clips.Count - 1;
        }
    }

    void RandomSong()
    {
        if (trackList.Count < clips.Count)
        {
            while (trackList.Contains(currentIndex))
            {
                currentIndex = Random.Range(0, clips.Count);
            }
            if (trackList.Count < 1)
            {
                while (prevIndex == currentIndex)
                    currentIndex = Random.Range(0, clips.Count);
            }
        }
        else
        {
            while (prevIndex == currentIndex)
                currentIndex = Random.Range(0, clips.Count);
            trackList.Clear();
        }
        PlayCurrent();
    }

    void PlayCurrent()
    {
        buttonUI[0].sprite = playButtonSprites[1];
        source.clip = clips[currentIndex];
        currSongText.text = source.clip.name.Remove(source.clip.name.Length - 4);
        if (backwards == 0)
            trackList.Add(currentIndex);
        source.Play();
        prevIndex = currentIndex;
    }

    void Pause()
    {
        buttonUI[0].sprite = playButtonSprites[0];
        source.Pause();
        paused = true;
    }

    void Resume()
    {
        buttonUI[0].sprite = playButtonSprites[1];
        source.UnPause();
        paused = false;
    }

    void ReloadSounds()
    {
        clips.Clear();
        buttonUI[0].sprite = playButtonSprites[0];
        songListText.text = origSongListText;
        // get all valid files
        DirectoryInfo info = new DirectoryInfo(absolutePath);
        soundFiles = info.GetFiles()
            .Where(f => IsValidFileType(f.Name)).ToList();
        foreach (FileInfo sFile in soundFiles)
        {
            if (Path.GetExtension(sFile.Name) == ".mp3")
            {
                ConvertMp3ToWav(sFile.FullName, sFile.FullName.Remove(sFile.FullName.Length - 4) + ".wav");
                File.Delete(sFile.FullName);
                //sFile.FullName = sFile.FullName.Remove(sFile.FullName.Length - 4) + ".wav";
                //soundFiles.Remove(sFile);
                //soundFiles.Add(info.GetFiles("*.wav").First());
                Debug.Log("WebCamDevice DirectSoundDeviceInfo it!");
            }
        }
        soundFiles = info.GetFiles()
    .Where(f => IsValidFileType(f.Name)).ToList();
        // and load them
        foreach (FileInfo s in soundFiles)
            StartCoroutine(LoadFile(s.FullName));
    }

    bool IsValidFileType(string fileName)
    {
        return validExtensions.Contains(Path.GetExtension(fileName));
        // Alternatively, you could go fileName.SubString(fileName.LastIndexOf('.') + 1); that way you don't need the '.' when you add your extensions
    }

    private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
    {
        using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
        {
            using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
            {
                WaveFileWriter.CreateWaveFile(_outPath_, pcm);
            }
        }
    }

    IEnumerator LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);
        print("loading " + path);

        AudioClip clip = www.GetAudioClip(false);
        while (clip.loadState != AudioDataLoadState.Loaded)
            yield return www;

        print("done loading");
        clip.name = Path.GetFileName(path);
        songListText.text += "\n" + clip.name.Remove(clip.name.Length - 4);
        clips.Add(clip);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{


    private Animator fadeAnimator;

    private float delay;
    private bool quit;
    public string sceneAfter;
    [SerializeField] public bool howPlay = false;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (!howPlay)
        {
            StartCoroutine(SceneDelay(sceneName));

        }
        else
        {
            //sceneAfter = sceneName;
            StartCoroutine(SceneDelay(sceneAfter));
            howPlay = false;
        }
    }


    public void LoadSceneByIndex(int sceneNumber)
    {
        StartCoroutine(SceneDelay(sceneNumber.ToString()));
    }

    public void SetDelay(float delaySet)
    {
        delay = delaySet;
    }



    private IEnumerator SceneDelay(string scene)
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(delay);
        if (!quit)
        {
            SceneManager.LoadScene(scene);
        }
        else
        {
            Application.Quit();
        }
    }

    public void Quit()
    {
        quit = true;
    }
}
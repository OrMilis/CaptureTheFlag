using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {

    public Animator animator;
    private int levelToLoad;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            MatchManagerScript.instance.setLevelChanger(this);
            MatchManagerScript.instance.setSpawns();
        }
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        if (SceneManager.GetActiveScene().buildIndex > 3)
            Destroy(MatchManagerScript.instance.gameObject);
        SceneManager.LoadScene(levelToLoad);
    }

}

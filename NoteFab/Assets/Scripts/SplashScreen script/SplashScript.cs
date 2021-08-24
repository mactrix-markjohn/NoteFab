using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {

            PlayFabSettings.TitleId = "B2867";

        }
        
        
        StartCoroutine(SplashWait());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator SplashWait()
    {
        
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(ConstantString.LoginScreen);
    }
}

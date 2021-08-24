using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    private string userEmail;
    private string userPassword;
    
    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {

            PlayFabSettings.TitleId = "B2867";

        }


        var request = new LoginWithEmailAddressRequest
        {
            Email = userEmail,
            Password = userPassword
        };
        
        PlayFabClientAPI.LoginWithEmailAddress(request,OnLoginSuccess, OnLoginFailure);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("Something went wrong");
    }
    
    
}

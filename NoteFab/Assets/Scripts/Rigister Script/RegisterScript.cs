using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterScript : MonoBehaviour
{

    public InputField UsernameField;
    public InputField EmailField;
    public InputField PasswordField;
    public Button SignupButton;
    public Button LoginButton;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        SignupButton.onClick.AddListener(() => { Register();});
        LoginButton.onClick.AddListener(() => { Login();});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Register()
    {

        if (String.IsNullOrEmpty(EmailField.text) || String.IsNullOrEmpty(PasswordField.text) || PasswordField.text.Length < 6 )
        {
            _ShowAndroidToastMessage("Email field or Password field is Empty or less than 6 characters.");
            return;
        }
        
        
        var request = new RegisterPlayFabUserRequest
        {
            Email = EmailField.text,
            Password = PasswordField.text,
            RequireBothUsernameAndEmail = false
        };

        
        PlayFabClientAPI.RegisterPlayFabUser(request,OnRegisterSuccess,OnError);
    }

    
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        _ShowAndroidToastMessage("Successfully registered");
        SceneManager.LoadScene(ConstantString.MainScreen);
    }
    
    void OnError(PlayFabError error)
    {
        _ShowAndroidToastMessage($"Error occurred during registration : {error.ErrorMessage}");
    }


    void Login()
    {
        
        SceneManager.LoadScene(ConstantString.LoginScreen);
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}

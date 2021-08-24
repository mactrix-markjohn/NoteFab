using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{

    public InputField EmailField;
    public InputField PasswordField;
    public Button ForgotPasswordButton;
    public Button LoginButton;
    public Button SignUpButton;
    





    // Start is called before the first frame update
    void Start()
    {
        //Add the onCLick event to the buttons
        
        ForgotPasswordButton.onClick.AddListener(() => { ForgetPassword(); });
        LoginButton.onClick.AddListener(() => { Login();});
        SignUpButton.onClick.AddListener(() => { SignUp(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void Login()
    {
        
        if (String.IsNullOrEmpty(EmailField.text) || String.IsNullOrEmpty(PasswordField.text) || PasswordField.text.Length < 6 )
        {
            _ShowAndroidToastMessage("Email field or Password field is Empty or less than 6 characters");
            return;
        }
        
        
        var request = new LoginWithEmailAddressRequest
        {
            Email = EmailField.text,
            Password = PasswordField.text
        };
        
        PlayFabClientAPI.LoginWithEmailAddress(request,OnLoginSuccess, OnLoginFailure);
        
    }
    
    
    
    private void OnLoginSuccess(LoginResult result)
    {
        _ShowAndroidToastMessage("Successfully logged in");
        SceneManager.LoadScene(ConstantString.MainScreen);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        _ShowAndroidToastMessage($"Something went wrong : {error.ErrorMessage}");
    }
    
    
    void ForgetPassword()
    {
        var request = new SendAccountRecoveryEmailRequest()
        {
            Email = EmailField.text,
            TitleId = "B2867"

        };
        
        PlayFabClientAPI.SendAccountRecoveryEmail(request,OnPasswordReset, OnError);
    }

    private void OnError(PlayFabError obj)
    {
        
    }

    private void OnPasswordReset(SendAccountRecoveryEmailResult obj)
    {
        _ShowAndroidToastMessage("Password reset message sent to your email");
    }


    void SignUp()
    {
        SceneManager.LoadScene(ConstantString.RegisterScreen);
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

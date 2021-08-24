using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

public class NoteScript : MonoBehaviour
{

    public Button Back;
    public Button Save;
    public Button Delete;
    public InputField Title;
    public InputField Body;
    public Text EditedDate;

   

    private ConstantString.MainSceneCallType calltype;

    public Dictionary<string, DataScript> notes;
    private DataScript note;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        AttachEvents();
        
        //Retrieve the Note Dictionary
        GetNote();

        calltype = MainScript.calltype; //TODO: change this once you implment it in MainScript
        
        
        //Check if the main scene call is for new note or existing note

        if (calltype == ConstantString.MainSceneCallType.ExistingNote)
        {
            //Existing note call

            note = MainScript.Notedata;
            FillNotePad(note);

        }
        else
        {
            //New note call
            //Nothing happens on start 

            EditedDate.text = DateTime.Now.ToString("dd MMMM yyyy");
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void AttachEvents()
    {

        Back.onClick.AddListener(() =>
        {
            //SaveNote();
            SceneManager.LoadScene(ConstantString.MainScreen);
        });
        
        
        Save.onClick.AddListener(() =>
        {
            
           SaveNote();
    
            
        });
        
        
        Delete.onClick.AddListener(() =>
        {
           DeleteNote();

            
        });
        
        
    }

    void SaveNote()
    {
        if (calltype == ConstantString.MainSceneCallType.ExistingNote)
        {
            // Existing note
            SaveExistingNote(note);
            
        }
        else
        {
            //New Note
            SaveNewNote();
        }
    }

    void DeleteNote()
    {
        if (calltype == ConstantString.MainSceneCallType.ExistingNote)
        {
            // Existing note
            DeleteExistingNote(note);
            SceneManager.LoadScene(ConstantString.MainScreen);
            
        }
        else
        {
            //New Note
            Body.text.Remove(0);
        }
    }


    // New Note Path Implementation
    #region NewNotePathImplementation

    void SaveNewNote(){

        //check if the Title and Body has content
        if (String.IsNullOrEmpty(Title.text) && String.IsNullOrEmpty(Body.text) )
        {
            ShowAndroidToastMessage("Both your Title and Body is Empty, please write something..");
            return;
        }
        
        //Set up the note dictionary and datascript
        string key = DateTime.Now.ToString();
        
        DataScript notedata = new DataScript(Title.text, Body.text, DateTime.Now.ToString("dd MMMM yyyy"),key);

        notes[key] = notedata;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>{{ConstantString.Notes,JsonConvert.SerializeObject(notes)}}

        };
        
        PlayFabClientAPI.UpdateUserData(request,OnDataSend,OnError);


    }

    private void OnError(PlayFabError obj)
    {
        ShowAndroidToastMessage($"An error occur with the message : {obj.ErrorMessage}");
    }

    private void OnDataSend(UpdateUserDataResult obj)
    {
        ShowAndroidToastMessage("Your note has been saved.");
    }


    #endregion

    
    
    
    // Existing Note Path Implementation
    #region ExistingNotePathImplementation


    void FillNotePad(DataScript notecontent)
    {
        Title.text = notecontent.Title;
        Body.text = notecontent.Body;
        EditedDate.text = notecontent.time;

    }

    void SaveExistingNote(DataScript note)
    {
        //check if the Title and Body has content
        if (String.IsNullOrEmpty(Title.text) && String.IsNullOrEmpty(Body.text) )
        {
            ShowAndroidToastMessage("Both your Title and Body is Empty, please write something..");
            return;
        }
        
        //Update the note dictionary and datascript

        
        string key = note.Key;
        
       note.Title = Title.text;
       note.Body = Body.text;
       note.Time = DateTime.Now.ToString("dd MMMM yyyy");
       
       notes[key] = note;

       var request = new UpdateUserDataRequest 
       {
            Data = new Dictionary<string, string>{{ConstantString.Notes,JsonConvert.SerializeObject(notes)}}
            
       };
        
       PlayFabClientAPI.UpdateUserData(request,OnDataSend,OnError);
        
        
    }


    void DeleteExistingNote(DataScript note)
    {
        //Update the note dictionary and datascript

        
        string key = note.Key;

        notes.Remove(key);

        var request = new UpdateUserDataRequest 
        {
            Data = new Dictionary<string, string>{{ConstantString.Notes,JsonConvert.SerializeObject(notes)}}
            
        };
        
        PlayFabClientAPI.UpdateUserData(request,OnDataDeleted,OnError);
    }

    private void OnDataDeleted(UpdateUserDataResult obj)
    {
        ShowAndroidToastMessage("Note has been removed");
    }

    #endregion
   

    



    void GetNote()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharactersDataReceived, OnError);
    }

    private void OnCharactersDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(ConstantString.Notes))
        {

            notes = JsonConvert.DeserializeObject<Dictionary<string, DataScript>>(result.Data[ConstantString.Notes]
                .Value);

        }
        else
        {
            notes = new Dictionary<string, DataScript>(); 
        }
    }


    
    
    private void ShowAndroidToastMessage(string message)
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

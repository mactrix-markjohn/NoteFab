using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{

    public InputField SearchField;
    public Button SearchButton;
    public Button AddNoteButton;
    public GameObject Noteitem;
    public GameObject ContentContainer;
    
    
    // Search panel and it gameobjects
    public GameObject SearchPanel;
    public Button SearchBackButton;
    public GameObject SearchScrollContent;
    public Text SearchResult;
    public Text ResultNotFoundText;
    
   
    public static ConstantString.MainSceneCallType calltype;
    
    public static Dictionary<string, DataScript> notes;
    public static DataScript Notedata;
    
    public List<DataScript> SearchedNoteList;
    public List<GameObject> SearchedObjectList;
    
    
    // Start is called before the first frame update
    void Start()
    {
        SearchedObjectList = new List<GameObject>();
        SearchPanel.SetActive(false);
        
        AttachListener();
        
        GetNote();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


  



    void AttachListener()
    {
        SearchButton.onClick.AddListener(() =>
        {
            
            SearchPlayFab(SearchField.text);
        });
        
        AddNoteButton.onClick.AddListener(() =>
        {
            calltype = ConstantString.MainSceneCallType.NewNote;
            SceneManager.LoadScene(ConstantString.NotePadScreen);
        });
        
        SearchField.onEndEdit.AddListener((string content) =>
        {
            OnEndEditAction(content);
            
        });
       
        //Onclick event for SearchBackButton
        SearchBackButton.onClick.AddListener(() => { SearchPanel.SetActive(false); });
        
    }
    
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
        
        //Populate the List with notes
        PopulateList(notes);
    }
    
    void PopulateList(Dictionary<string, DataScript> notedata)
    {

        foreach (var note in notedata)
        {

            GameObject noteitem = Instantiate(Noteitem, ContentContainer.transform);
            noteitem.GetComponent<NoteListScript>().Setup(note.Value.Title,note.Value.Body, note.Value,this);
            
            



        }
        
    }
    
    //Call method from the list items

    public void NoteitemClickEvent(DataScript note)
    {
        calltype = ConstantString.MainSceneCallType.ExistingNote;

        Notedata = note;

        //Load the Notepad scene
        SceneManager.LoadScene(ConstantString.NotePadScreen);

    }
    
   


    void OnEndEditAction(string content)
    {
        bool pass = false;
        if (TouchScreenKeyboard.isSupported)
        {
            if (SearchField.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                pass = true;
            }
        }
 
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Submit"))
        {
            pass = true;
        }
 
        if (!pass)
        {
            return;
        }
        
        SearchPlayFab(content);
        
        
        //TODO: Call the search Algorithm here
        
    }
    
   

    


    #region SearchPanelRegion

    
    
    void SearchPlayFab(string content)
    {
        SearchedNoteList = new List<DataScript>(); // instantiate the search list


        foreach (var searchobject in SearchedObjectList)
        {
            Destroy(searchobject);
        }
        
        ResultNotFoundText.gameObject.SetActive(false);
        
       
        
        //Loop through content of notes and find the notes that content the search word in Title and Body
        //and add the note to the list

        foreach (var note in notes)
        {
            if (note.Value.Title.ToLower().Contains(content.ToLower()) || note.Value.Body.ToLower().Contains(content.ToLower()))
            {
                DataScript searchedNote = note.Value;
                
                SearchedNoteList.Add(searchedNote);
            }
        }

        if (SearchedNoteList.Count > 0)
        {
            SearchResult.text = $"Search Result : {SearchedNoteList.Count}";
            SpawnSearchObject(SearchedNoteList);
            
        }
        else
        {
            SearchResult.text = $"Search Result : {SearchedNoteList.Count}";
            ResultNotFoundText.gameObject.SetActive(true);
        }
        
        //Make Search Panel visible
        SearchPanel.SetActive(true);





    }


    void SpawnSearchObject(List<DataScript> notelist)
    {
        SearchedObjectList.Clear(); // clear up the list to add new one

        foreach (var note in notelist)
        {
            
            GameObject noteitem = Instantiate(Noteitem, SearchScrollContent.transform);
            noteitem.GetComponent<NoteListScript>().Setup(note.Title,note.Body, note,this);
            
            SearchedObjectList.Add(noteitem);
            
        }
        
    }


    #endregion
    
    
    private void OnError(PlayFabError obj)
    {
        ShowAndroidToastMessage($"An error occur with the message : {obj.ErrorMessage}");
    }

    private void OnDataSend(UpdateUserDataResult obj)
    {
        ShowAndroidToastMessage("Your note has been saved.");
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

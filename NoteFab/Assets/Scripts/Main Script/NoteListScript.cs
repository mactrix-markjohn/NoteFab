using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class NoteListScript : MonoBehaviour
{

    public Text Title;
    public Text Body;
    private MainScript script;
    public DataScript note;
    public Button noteClick;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(string title, string body,DataScript noteitem, MainScript mainScript)
    {
        Title.text = title;
        Body.text = body;
        script = mainScript;
        note = noteitem;
        
        
        noteClick.onClick.AddListener(() => { HandleClick();});
    }

    void HandleClick()
    {
        script.NoteitemClickEvent(note);
    }


}

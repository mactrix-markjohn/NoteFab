using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScript
{

    public string title;
    public string body;
    public string time;
    public string key;

    public DataScript(string title, string body, string time, string key)
    {
        this.title = title;
        this.body = body;
        this.time = time;
        this.key = key;
    }

    public string Title
    {
        get => title;
        set => title = value;
    }

    public string Body
    {
        get => body;
        set => body = value;
    }
    
    public string Time
    {
        get => time;
        set => time = value;
    }
    
    
    public string Key
    {
        get => key;
        set => key = value;
    }
    
    
    
    
    
    

   
}

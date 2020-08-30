using UnityEngine;
using System.Collections;
using System.IO;

public class Customization : MonoBehaviour 
{
    public static Customization instance;

    void Start()
    {
        instance = this;
    }

    public static IEnumerator LoadTexture2D(string path, Texture2D image) 
    {
        WWW imageLink = null;        
        
#if UNITY_STANDALONE || UNITY_EDITOR_WIN
        imageLink = new WWW("file:///" + path);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        imageLink = new WWW("file://" + path);
#endif

        yield return imageLink;

        if (imageLink.error == null && imageLink.isDone)
        {            
            image.LoadImage(imageLink.bytes);                      
        }         
    }

    public static Texture2D GetTexture2DFromLink(string link)
    {
        WWW imageLink = null;

#if UNITY_STANDALONE || UNITY_EDITOR_WIN
        imageLink = new WWW("file:///" + link);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        imageLink = new WWW("file://" + link);
#endif        
        return imageLink.texture;                 
    }

    public static int GetIntFromFile(string path)
    {
        string value = File.ReadAllText(path, System.Text.Encoding.UTF8);                        

        return int.Parse(value);
    }

    public static void WriteIntOnFile(string path, int value) 
    {
        string newValue = value.ToString();
        File.WriteAllText(path, newValue);
    }   
}
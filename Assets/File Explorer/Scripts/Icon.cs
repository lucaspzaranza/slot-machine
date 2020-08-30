using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

public enum IconType
{
    Folder,
    File,
}

public class Icon : MonoBehaviour
{
    #region Properties
    public string iconName { get; set; }
    public string iconCompleteName { get; set; }
    public string iconAdress { get; set; }
    public bool isSelected { get; set; }
    public bool isMouseOver { get; set; }
    public int clickCount { get; set; }
    private bool isShowingCompleteName { get; set; }
    public IconType iconType { get; private set; }
    public bool isAccessible { get; set; }

    #endregion

    #region Variables

    public float timeToResetClickCount;
    private float timeCount;
    private Transform textChild;    
    public GameObject selectedPanel;
    private float timeToShowCompleteName;
    public GameObject completeNameWindow;
    private GameObject windowGameObject;
    private FileExplorer fileExplorer;
    public GameObject prohibitedGameObject;    

    #endregion

    void Start()
    {
        textChild = gameObject.transform.GetChild(0);

        fileExplorer = FindObjectOfType<FileExplorer>();
      
        SetCurrentType(gameObject.tag);

        iconAdress = FileExplorer.currentAdress.FullName + "\\" + iconCompleteName;

        isAccessible = FolderIsAcessible(iconAdress);

        if (iconType == IconType.Folder && !isAccessible)
            prohibitedGameObject.SetActive(true);
    }
    
    void Update()
    {
        if(isSelected)
            timeCount += Time.deltaTime;

        if(isSelected && timeCount >= timeToResetClickCount)
        {
            clickCount = 0;
            timeCount = 0;
        }                

        if(Input.GetMouseButton(0) && !isMouseOver)
        {
            DeselectIcon();
        }

        if(isMouseOver)
        {
            timeToShowCompleteName += Time.deltaTime;

            if(timeToShowCompleteName >= 1f && !isShowingCompleteName)
            {                
                ShowCompleteIconName();
                timeToShowCompleteName = 0;
                isShowingCompleteName = true;
            }
        }
    }

    private bool FolderIsAcessible(string path)
    {
        try
        {
            Directory.GetDirectories(path);          
        }
        catch (System.UnauthorizedAccessException)
        {           
            return false;
        }

        return true;
    }

    private void SetCurrentType(string typeOfIcon)
    {
        try
        {            
            iconType = (IconType) Enum.Parse(typeof(IconType), typeOfIcon, true);
        }
        catch
        {
            iconType = IconType.Folder;
        }       
    }

    public void IncrementClickCount()
    {
        clickCount++;  
        if(clickCount == 2)
        {
            fileExplorer.OpenSelectedFolder(FileExplorer.selectedFolder.iconAdress, false);            
            ResetIconData();
        }        
    }

    public void TogglePanelActivation(bool toggle)
    {
        selectedPanel.SetActive(toggle);

        if(toggle == false && isSelected)
        {
            selectedPanel.SetActive(true);
        }       
    }
  
    private void ResetIconData()
    {
        isSelected = false;
        timeCount = 0;
        clickCount = 0;
    }

    public void SetSelectedIcon()
    {               
        iconName = textChild.gameObject.GetComponent<TextMesh>().text;                       
        FileExplorer.selectedFolder = this;       
        isSelected = true;        
    }

    public void DeselectIcon()
    {
        FileExplorer.instance = null;       
        ResetIconData();
        TogglePanelActivation(false);       
    }

    private void ShowCompleteIconName()
    {        
        if(iconName.EndsWith("...")) 
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = 0;
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, -5.4f, 5.4f);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y + 0.2f, -4f, 4f);
            windowGameObject = Instantiate(completeNameWindow, cursorPosition, Quaternion.identity) as GameObject;

            GameObject windowBackground = windowGameObject.transform.GetChild(0).gameObject;

            windowGameObject.GetComponent<TextMesh>().text = fileExplorer.FormatCompleteName(iconCompleteName);
            windowGameObject.AddComponent<BoxCollider>();
            BoxCollider windowCollider = windowGameObject.GetComponent<BoxCollider>();
            Vector3 colliderDimensions = windowCollider.size;
            windowCollider.enabled = false;
            windowBackground.transform.localScale = new Vector3(colliderDimensions.x / 2.2f, colliderDimensions.y / 2, colliderDimensions.z);             
        }        
    }     

    void OnMouseDown()
    {
        IncrementClickCount();
        if(!isSelected)
            SetSelectedIcon();       
    }

    void OnMouseEnter()
    {
        TogglePanelActivation(true);
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        TogglePanelActivation(false);
        isMouseOver = false;

        if(isShowingCompleteName)
        {
            Destroy(windowGameObject);
        }

        isShowingCompleteName = false;
        timeToShowCompleteName = 0;
    }   
}
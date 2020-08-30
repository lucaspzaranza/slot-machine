using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ExplorerUI : MonoBehaviour
{
    #region Variables

    public Scrollbar scrollbar;
    public GameObject rowsParent;    

    public Button backButton;
    public Button upButton;
    public Button okButton;
    
    public FileExplorer fileExplorer;

    public delegate void CallBackToPreviousMenu();

    public CallBackToPreviousMenu callBackToPreviousMenu;

    public GameUI gameUI;
   
    #endregion

    void Start()
    {
        fileExplorer = FindObjectOfType<FileExplorer>();

        callBackToPreviousMenu = gameUI.BackToSettingsMenu;
    }

    void Update()
    {
        if (FileExplorer.currentAdress.FullName == FileExplorer.rootFolder.FullName)        
            upButton.gameObject.SetActive(false);
                    
        else        
            upButton.gameObject.SetActive(true);      
                     
        if(FileExplorer.directoriesList.Count > 1 && !backButton.isActiveAndEnabled)       
            backButton.gameObject.SetActive(true);
               
    }

    public IEnumerator SetScrollbarSize()
    {        
        yield return new WaitForEndOfFrame();
        float value = ((rowsParent.transform.childCount - 5) / 12.5f);
        value = Mathf.Clamp(value, 0.00001f, 0.99999f);
        scrollbar.size = 1 - value;        
    }

    public void ChangeScrollView()
    {
        float valueVariation;
        int childCount = rowsParent.transform.childCount - fileExplorer.initialRowChildCount;
        valueVariation = childCount * 0.475f;

        rowsParent.transform.localPosition = new Vector3(0, scrollbar.value * valueVariation, 0);        
    } 

    public void BackToParentDirectory()
    {
        DirectoryInfo parent = FileExplorer.currentAdress.Parent;
        fileExplorer.OpenSelectedFolder(parent.FullName, true);

        if (fileExplorer.isInstantiating)
            fileExplorer.ResetExplorerData();
    }

    public void BackToPreviousDirectory()
    {
        LinkedListNode<DirectoryInfo> currentNode = FileExplorer.directoriesList.Find(FileExplorer.currentAdress);
        fileExplorer.OpenSelectedFolder(currentNode.Previous.Value.FullName, true);

        if (fileExplorer.isInstantiating)
            fileExplorer.ResetExplorerData();
    }

    public void CallSetFolderOfFileAdress()
    {
        fileExplorer.SetFolderOfFileAdress();
    }

    public void BackToPreviousMenu()
    {
        callBackToPreviousMenu();
    }
}
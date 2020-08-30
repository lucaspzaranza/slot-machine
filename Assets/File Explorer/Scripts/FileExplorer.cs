using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class FileExplorer : MonoBehaviour
{
    #region Variables

    public GameObject folderPrefab;
    public GameObject filePrefab;
    public GameObject rowsParent;   
    public GameObject rowPrefab;
    public GameObject unauthorizedAccesWindow;
    public GameObject fileNotFoundWindow;
    
    public static FileExplorer instance;

    public Scrollbar scrollbar;
    public ExplorerUI gameUIScript;

    public int numOfCharsInARow;
    public float foldersRowHeightDifference;

    private bool folderHasManyUpperChars;

    private int rowIndex = 0;
    private int childIndex = 0;

    public bool isInstantiating;

    public bool showFiles;

    public string currentPath;
    public string selectedPath;

    #endregion

    #region Properties

    public static DirectoryInfo currentAdress { get; private set; }
    public static DirectoryInfo rootFolder { get; set; }
    public static DirectoryInfo parentFolder { get; set; }
    public static Icon selectedFolder { get; set; }
    public int initialRowChildCount { get; private set; }
    public static LinkedList<DirectoryInfo> directoriesList { get; set; }
    public static string selectedAdress { get; set; }

    #endregion

    // Use this for initialization
	void Start() 
    {
        instance = this;       
	}	
       
    void Update()
    {
        currentPath = currentAdress.FullName;
        try
        {
            selectedPath = selectedFolder.name;
        }
        catch
        {

        }        
    }
      
    public void OpenSelectedFolder(string adress, bool isParentFolder)
    {       
        if(selectedFolder.iconType == IconType.Folder)
        {
            if (isParentFolder || selectedFolder.isAccessible)
            {
                currentAdress = new DirectoryInfo(adress);
                DestroyIcons();
                ResetExplorerData();
                InstantiateFilesAndFolders();
                UpdateDirectoriesList();
            }
            else
                StartCoroutine(ActivateUnauthorizedAccessWindow());
        }
    }
  
    public void UpdateDirectoriesList()
    {        
        directoriesList.AddLast(currentAdress);
    }

    private IEnumerator ActivateUnauthorizedAccessWindow()
    {
        unauthorizedAccesWindow.SetActive(true);
        yield return new WaitForSeconds(3f);
        unauthorizedAccesWindow.SetActive(false);
    }

    public void ActivateFileNotFoundWindow()
    {
        fileNotFoundWindow.SetActive(true);         
    }

    private void DestroyIcons()
    {
        Icon[] icons = FindObjectsOfType<Icon>();
        

        foreach (Icon icon in icons)
        {
            Destroy(icon.gameObject);
        }

        for (int i = initialRowChildCount; i < rowsParent.transform.childCount; i++)
        {
            Transform child = rowsParent.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        if (scrollbar.gameObject.activeInHierarchy)
        {           
            scrollbar.gameObject.SetActive(false);            
        }            

        GameObject completeGameWindow = GameObject.FindGameObjectWithTag("Complete Name Window");
        if(completeGameWindow != null)
        {
            Destroy(completeGameWindow);
        }
    }    

    public void ResetExplorerData()
    {
        rowsParent.transform.localPosition = Vector3.zero;
        childIndex = 0;
        rowIndex = 0;
        isInstantiating = false;
    }

    public void InstantiateFilesAndFolders()
    {
        FileSystemInfo[] files;
        FileSystemInfo[] folders;
        GetFoldersInPath(currentAdress.FullName, out folders);
        GetFilesInPath(currentAdress.FullName, out files);
        
        if (folders.Length <= 2000)
            InstantiateIcons(ref folders);
        else
            StartCoroutine(InstantiateIcons(folders));

        if(showFiles)
        {
            if (files.Length <= 2000)
                InstantiateIcons(ref files);
            else
                StartCoroutine(InstantiateIcons(files));
        }        
    }

    private void GetFoldersInPath(string path, out FileSystemInfo[] directoriesInfo)
    {
        string[] foldersNames = Directory.GetDirectories(path);
        directoriesInfo = new DirectoryInfo[foldersNames.Length];

        for (int i = 0; i < foldersNames.Length; i++)
        {
            directoriesInfo[i] = new DirectoryInfo(foldersNames[i]);
        }
    }

    private void GetFilesInPath(string path, out FileSystemInfo[] filesInfo)
    {
        string[] filesNames = Directory.GetFiles(path);
        filesInfo = new FileInfo[filesNames.Length];

        for (int i = 0; i < filesNames.Length; i++)
        {
            filesInfo[i] = new FileInfo(filesNames[i]);       
        }
    }
  
    private void InstantiateIcons(ref FileSystemInfo[] iconsInfo)
    {
        GameObject iconGameObject = null;             
       
        foreach (FileSystemInfo icon in iconsInfo)
        {
            Transform iconChild;
            TextMesh textComponent;
            Transform row = null;
            Transform rowChild;
            GameObject newRow = null;

            if (iconsInfo[0].GetType() == typeof(FileInfo))
                iconGameObject = Instantiate(filePrefab) as GameObject;

            else if (iconsInfo[0].GetType() == typeof(DirectoryInfo))
                iconGameObject = Instantiate(folderPrefab) as GameObject;

            iconChild = iconGameObject.transform.GetChild(0);

            try
            {
                row = rowsParent.transform.GetChild(rowIndex);
            }
            catch
            {
                if (!scrollbar.gameObject.activeInHierarchy)
                {
                    scrollbar.gameObject.SetActive(true);
                    scrollbar.value = 0;
                    StartCoroutine(gameUIScript.SetScrollbarSize());
                }

                newRow = Instantiate(rowPrefab) as GameObject;
                newRow.transform.SetParent(rowsParent.transform, false);

                row = rowsParent.transform.GetChild(rowIndex - 1);
                float lastChildY = row.transform.localPosition.y;
                newRow.transform.localPosition = new Vector3(0, lastChildY - 1f, 0);

                int panelChildCount = rowsParent.transform.childCount;
                newRow.name = "Row " + panelChildCount.ToString();
            }

            if (newRow == null)
                rowChild = row.GetChild(childIndex);
            else
            {
                rowChild = newRow.transform.GetChild(childIndex);
                newRow = null;
            }

            iconGameObject.transform.SetParent(rowChild, true);
            iconGameObject.transform.localPosition = new Vector3(0, 0, 0);
            childIndex++;

            textComponent = iconChild.gameObject.GetComponent<TextMesh>();
            textComponent.text = icon.Name;
            iconGameObject.name = icon.Name;
            iconGameObject.GetComponent<Icon>().iconCompleteName = icon.Name;

            FormatFolderName(textComponent);
            iconGameObject.GetComponent<Icon>().iconName = textComponent.text;

            if (childIndex == 8)
            {
                rowIndex++;
                childIndex = 0;
            }
        }                   
    }

    private IEnumerator InstantiateIcons(FileSystemInfo[] iconsInfo)
    {
        GameObject iconGameObject = null;

        isInstantiating = true;
               
        foreach (FileSystemInfo icon in iconsInfo)
        {
            if (isInstantiating)
            {
                Transform iconChild;
                TextMesh textComponent;
                Transform row = null;
                Transform rowChild;
                GameObject newRow = null;

                yield return new WaitForEndOfFrame();

                if (iconsInfo[0].GetType() == typeof(FileInfo))
                    iconGameObject = Instantiate(filePrefab) as GameObject;

                else if (iconsInfo[0].GetType() == typeof(DirectoryInfo))
                    iconGameObject = Instantiate(folderPrefab) as GameObject;

                iconChild = iconGameObject.transform.GetChild(0);

                try
                {
                    row = rowsParent.transform.GetChild(rowIndex);
                }
                catch
                {
                    if (!scrollbar.gameObject.activeInHierarchy)
                    {
                        scrollbar.gameObject.SetActive(true);
                        StartCoroutine(gameUIScript.SetScrollbarSize());
                    }
                    else
                    {                        
                        StartCoroutine(gameUIScript.SetScrollbarSize());
                    }

                    newRow = Instantiate(rowPrefab) as GameObject;
                    newRow.transform.SetParent(rowsParent.transform, false);

                    row = rowsParent.transform.GetChild(rowIndex - 1);
                    float lastChildY = row.transform.localPosition.y;
                    newRow.transform.localPosition = new Vector3(0, lastChildY - 1f, 0);

                    int panelChildCount = rowsParent.transform.childCount;
                    newRow.name = "Row " + panelChildCount.ToString();
                }

                if (newRow == null)
                    rowChild = row.GetChild(childIndex);
                else
                {
                    rowChild = newRow.transform.GetChild(childIndex);
                    newRow = null;
                }

                iconGameObject.transform.SetParent(rowChild, true);
                iconGameObject.transform.localPosition = new Vector3(0, 0, 0);
                childIndex++;

                textComponent = iconChild.gameObject.GetComponent<TextMesh>();
                textComponent.text = icon.Name;
                iconGameObject.name = icon.Name;
                iconGameObject.GetComponent<Icon>().iconCompleteName = icon.Name;

                FormatFolderName(textComponent);
                iconGameObject.GetComponent<Icon>().iconName = textComponent.text;

                if (childIndex == 8)
                {
                    rowIndex++;
                    childIndex = 0;
                }
            }
            else yield break;                
        }             
    }

    private void FormatFolderName(TextMesh folderName)
    {        
        string nameString = folderName.text;
        string[] words = folderName.text.Split(new char[] { ' ' });
        
        int rowLength = numOfCharsInARow;

        if (HasManyUpperChars(folderName.text))
            rowLength -= 3;  

        folderName.text = string.Empty;

        if (words.Length == 1)
            folderName.text = FormatName(nameString, rowLength);
        else
            folderName.text = FormatNames(ref words, rowLength);

        //print(folderName.text + " = " + folderName.GetComponent<Renderer>().bounds.extents);
    }   

    private string FormatName(string wordToFormat, int rowLength)
    {
        int startIndex = 0;
        string nameString = wordToFormat;        
        string wordFormatted = string.Empty;
        int maxLength = rowLength * 3;                        

        wordToFormat = string.Empty;
          
        try 
        {
            for (int i = 0; i < nameString.Length; i += rowLength)
            {
                string nameSubstring = nameString.Substring(startIndex, rowLength);                
                wordToFormat += nameSubstring + System.Environment.NewLine;
                startIndex += rowLength;
            }
        }
        catch
        {
            string nameSubstring = nameString.Substring(startIndex, nameString.Length - startIndex);
            wordToFormat += nameSubstring;                
        }

        if (wordToFormat.Length > maxLength)
        {         
            wordFormatted = SetThreePointsAtFinal(wordToFormat, rowLength);            
        }   
        else
        {
            wordFormatted = wordToFormat;
        }

        return wordFormatted;
    }
 
    private string FormatNames(ref string[] words, int rowLength)
    {
        string formattedWord = string.Empty;
        int rowRemainingChars = numOfCharsInARow;
        int maxLength = rowLength * 3; 

        for (int i = 0; i < words.Length; i++)
        {
            int currentLength = words[i].Length;

            if (currentLength < rowRemainingChars + 1)
            {
                rowRemainingChars -= currentLength;
                formattedWord += words[i] + " ";
            }
            else
            {
                rowRemainingChars = numOfCharsInARow;

                formattedWord = formattedWord.Trim();              
                if (currentLength > rowRemainingChars)
                {
                    formattedWord += System.Environment.NewLine + FormatName(words[i], rowLength) + " ";
                }       
                else
                {
                    formattedWord += System.Environment.NewLine + words[i] + " ";
                }
                rowRemainingChars -= currentLength;
            }
        }        

        if (formattedWord.Length > maxLength)
        {
            formattedWord = SetThreePointsAtFinal(formattedWord, rowLength);
        }       

        return formattedWord.Trim();
    }

    public string FormatCompleteName(string wordToFormat)
    {       
        string[] words = wordToFormat.Split(new char[] {' '});
        string formattedWord = string.Empty;
        int rowRemainingChars = numOfCharsInARow * 3;
        int rowLength = rowRemainingChars;

        for (int i = 0; i < words.Length; i++)
        {
            int currentLength = words[i].Length;

            if (currentLength < rowRemainingChars + 1)
            {
                rowRemainingChars -= currentLength;
                formattedWord += words[i] + " ";
            }
            else
            {
                rowRemainingChars = rowLength;

                formattedWord = formattedWord.Trim();
                if (currentLength > rowRemainingChars)
                {
                    formattedWord += System.Environment.NewLine + FormatName(words[i], rowLength) + " ";
                }
                else
                {
                    formattedWord += System.Environment.NewLine + words[i] + " ";
                }
                rowRemainingChars -= currentLength;
            }
        }

        return formattedWord.Trim();
    }

    private string SetThreePointsAtFinal(string wordToFormat, int rowLength)
    {        
        string threePoints = "...";
        string wordFormatted = string.Empty;
        int maxLength = (rowLength * 3) - 1;

        wordFormatted = wordToFormat.Substring(0, maxLength);
        wordFormatted = wordFormatted.Insert(maxLength, threePoints);
        
        return wordFormatted;
    }

    private bool HasManyUpperChars(string stringToCheck)
    {
        int numOfUpperChars = 0;

        for (int i = 0; i < stringToCheck.Length; i ++)
        {
            if(char.IsUpper(stringToCheck, i))
            {
                numOfUpperChars++;
            }
        }

        if(numOfUpperChars >= numOfCharsInARow / 3)
        {
            return true;
        }

        return false;
    }
   
    public void SetFolderOfFileAdress()
    {
        if (currentAdress != null && currentAdress != rootFolder && selectedFolder != null && selectedFolder.isAccessible)
        {            
            selectedAdress = currentAdress.FullName + "\\" + selectedFolder.iconCompleteName;
        }            
        else
        {            
            selectedAdress = rootFolder.FullName;          
        }           
    }    

    void OnEnable()
    {        
        initialRowChildCount = rowsParent.transform.childCount;

        currentAdress = new DirectoryInfo(Application.dataPath);
        currentAdress = currentAdress.Root;
       
        try
        {
            parentFolder = Directory.GetParent(currentAdress.FullName);
            rootFolder = parentFolder.Root;
        }
        catch
        {
            parentFolder = currentAdress;
            rootFolder = currentAdress;
        }

        directoriesList = new LinkedList<DirectoryInfo>();
        directoriesList.AddFirst(currentAdress);

        InstantiateFilesAndFolders();        
    }

    void OnDisable()
    {
        DestroyIcons();
        ResetExplorerData();

        directoriesList.Clear();
    }   
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System;

public class SlotMachineController : MonoBehaviour
{
    #region Variables

    public GameObject[] prizes;
    public GameObject[] rows;   
    public RowData[] rowScript;    
    public GameObject desacelerator;
    public bool slotMachineOn;
    public bool isRandom = false;
    public float rowVelocity;  
    public Dictionary<GameObject, int> prizesDictionary;
    public GameObject prizePrefab;
    public GameObject prizeChosen;
    public GameObject settingsButton;
    public Dictionary<GameObject, GameObject> prizeInputTextFieldDictionary;
    public GameObject settingsPanel;
    public GameObject quantityPanel;
    public GameObject slotMachineSpriteGameObject;
    
    public int bounciness;

    //[SerializeField] Mostra variáveis privadas no Editor. xD
    private int frameCount;

    public float handleGravity; 
    public string customFolderDataPath;

    public bool isCustomized = false;

    public GameUI gameUI;

    public FileInfo[] files;

    public bool changedCustomFolder;

    public FileExplorer fileExplorer;

    public bool isTrial;

    private TimeSpan totalTime;
    private DateTime initialDate;
    private DateTime currentDate;

    public int daysToExpire;
         
    #endregion

    #region Properties
    public float deadzone { get; private set; }
    public float heightDifference { get; private set; }
    private bool updateCalled { get; set; }
    private int folderFileCount { get; set; }
    public bool isExpired 
    { 
        get
        {
            return totalTime.TotalDays > daysToExpire;           
        }
    }
    public bool isOurComputer
    {
        get
        {
            return  File.Exists("C:/Windows/key.txt");
        }
    }

    #endregion 
    
    // Use this for initialization
	void Start()
    {       
        deadzone = 7.5f;
        heightDifference = prizes[1].transform.localPosition.y - prizes[0].transform.localPosition.y;

        Physics.gravity = -Physics.gravity;
        Physics.gravity = new Vector3(Physics.gravity.x, handleGravity, Physics.gravity.z);

        customFolderDataPath = string.Empty;       
        customFolderDataPath = Application.dataPath + "/Custom";

        prizesDictionary = new Dictionary<GameObject, int>();
        SetPrizesQuantity();
        SetPrizesColliderSize();

        if(isTrial)
        {           
            initialDate = new DateTime(2015, 7, 15);            
            currentDate = DateTime.Today;           
            totalTime = currentDate - initialDate;
                        
            if (isExpired)
            {
                gameUI.CallExpiredPanel();
            }
        }

        if(!isOurComputer)
        {
            //gameUI.CallPiracyPanel();
        }
    }
	
	// Update is called once per frame
	void Update()
    {      
        if (!string.IsNullOrEmpty(customFolderDataPath) && Directory.Exists(customFolderDataPath))
        {                   
            if (frameCount == 10)
            {
                if (FileExistInPath(customFolderDataPath))
                {
                    isCustomized = true;
                    UpdateGraphics();
                    SetPrizesColliderSize();                    
                }            
                else
                {                    
                    fileExplorer.ActivateFileNotFoundWindow();
                }                
            }                       
        }

        if(isCustomized && gameObject.activeInHierarchy && !updateCalled && frameCount == 8)
        {
            UpdatePrizesQuantity();
            updateCalled = true;
        }
        
        if (slotMachineOn)
        {
            MoveSlots();
        }

        frameCount++;
	}
    
    public void ResetFrameCount()
    {
        frameCount = 0;
    }

    private void GetSlotMachineSprite()
    {
        string spritePath = string.Format("{0}/background.png", customFolderDataPath);
        SpriteRenderer spriteRenderer = slotMachineSpriteGameObject.GetComponent<SpriteRenderer>();
        Texture2D image = null;

        if(File.Exists(spritePath))
        {
            image = new Texture2D(1920, 1080, TextureFormat.ARGB32, true);
            StartCoroutine(Customization.LoadTexture2D(spritePath, image));
            Rect imageRect = new Rect(0, 0, 1920, 1080);
            spriteRenderer.sprite = Sprite.Create(image, imageRect, new Vector2(0.5f, 0.5f));
        }

        //print(Screen.width + " x " + Screen.height);
    }

    private void UpdateGraphics()
    {                       
        folderFileCount = GetFolderFileCount(customFolderDataPath);

        bool destroyRemainingPrizes = folderFileCount < prizes.Length;

        prizesDictionary = new Dictionary<GameObject, int>();

        if (destroyRemainingPrizes) 
        {
            foreach(GameObject row in rows)
            {
                for (int i = folderFileCount; i < prizes.Length; i++)
                {
                    Destroy(row.transform.GetChild(i).gameObject);
                }
            }            
        }

        prizes = new GameObject[folderFileCount];

        GetSlotMachineSprite();

        for (int i = 0; i <= folderFileCount - 1; i++)
        {            
            string prizePath = string.Empty;            

            prizePath = string.Format("{0}/premio {1}.png", customFolderDataPath, i + 1);
                                               
            foreach (GameObject row in rows)
            {
                SpriteRenderer newSpriteRenderer = null;
                Texture2D image = null;

                try
                {
                    GameObject child = row.transform.GetChild(i).gameObject;
                    newSpriteRenderer = child.GetComponent<SpriteRenderer>();
                    image = newSpriteRenderer.sprite.texture;
                    prizes[i] = child;
                }
                catch
                {
                    RowData rowScript = row.GetComponent<RowData>();
                    GameObject newPrize = Instantiate(prizePrefab) as GameObject;
                    Vector3 topmost = rowScript.GetTopMostElement().transform.position;
                    newSpriteRenderer = newPrize.GetComponent<SpriteRenderer>();

                    newPrize.transform.SetParent(row.transform, true);
                    newPrize.GetComponent<Collider>().isTrigger = true;

                    image = newSpriteRenderer.sprite.texture;

                    newPrize.transform.position = new Vector3(topmost.x, topmost.y + heightDifference, topmost.z);

                    rowScript.prizesList.AddFirst(newPrize);

                    prizes[i] = newPrize;
                }

                prizes[i].name = string.Format("prize {0}", i + 1);

                image = new Texture2D(350, 450, TextureFormat.ARGB32, true);
                StartCoroutine(Customization.LoadTexture2D(prizePath, image));
                Rect imageRect = new Rect(0, 0, image.width, image.height);
                newSpriteRenderer.sprite = Sprite.Create(image, imageRect, new Vector2(0.5f, 0.5f));
            }

            prizesDictionary.Add(prizes[i], 0);
        }                          
    }

    public bool FileExistInPath(string path)
    {       
        DirectoryInfo currentPath = new DirectoryInfo(path);
        int fileCount = 0;

        files = currentPath.GetFiles();

        foreach (FileInfo file in files)
        {
            if (file.Name.StartsWith("premio") && file.Name.EndsWith(".png"))
            {                             
                fileCount++;
                break;
            }                    
        }

        return fileCount > 0;           
    }

    private int GetFolderFileCount(string path)
    {
        DirectoryInfo currentPath = new DirectoryInfo(path);
        int fileCount = 0;

        files = currentPath.GetFiles();
        
        foreach (FileInfo file in files)
        {
            if (file.Name.StartsWith("premio") && file.Name.EndsWith(".png"))
            {
                fileCount++;               
            }
        }

        return fileCount;
    }

    private bool PrizesMatch()
    {
        bool logic = true;

        for (int i = 0; i < rowScript.Length - 1; i++)
        {
            logic &= rowScript[i].randomPrize.name == rowScript[i + 1].randomPrize.name;
        }
            
        return logic;
    }

    public void ActivateSlotMachine()
    {
        if (!slotMachineOn)
        {
            slotMachineOn = true;
            gameUI.gameStates = GameStates.SlotMoving;

            if (isRandom)
                ChooseSlotsPrizesRandomly();
            else
                ChoosePrize();

            StartCoroutine(SetRowsDesaceleration());
            SetRowsVelocity(rowVelocity);
        }
    }

    private void ChoosePrize()
    {
        int randomPrize = UnityEngine.Random.Range(0, prizes.Length);       
        int numOfLoops = 0;       
       
        while (!ExistPrize(prizes[randomPrize]))
        {
            if (numOfLoops < prizes.Length)
            {
                randomPrize++;
                if (randomPrize >= prizes.Length)
                    randomPrize = 0;
            }
            else
            {
                print("Premios esgotados! Escolhendo um premio qualquer...");
                randomPrize = UnityEngine.Random.Range(0, prizes.Length);
                break;
            }
            numOfLoops++;
        }

        if (prizesDictionary[prizes[randomPrize]] > 0)
        {
            prizesDictionary[prizes[randomPrize]]--;

            if (prizeInputTextFieldDictionary != null && prizeInputTextFieldDictionary.ContainsKey(prizes[randomPrize]))                                   
                PlayerPrefs.SetInt("Item" + randomPrize, prizesDictionary[prizes[randomPrize]]);                                                            
        } 
                           
        prizeChosen = prizes[randomPrize];                       
    }
   
    private bool ExistPrize(GameObject currentPrize)
    {            
        return prizesDictionary[currentPrize] > 0;
    }

    private string GetTextFileName(GameObject currentPrize)
    {
        string currentName = currentPrize.name;
        string newName = currentName.Replace("prize", "premio");
        
        return newName;
    }

    private void ChooseSlotsPrizesRandomly()
    {
        foreach (RowData row in rowScript)
        {
            int randomPrize = UnityEngine.Random.Range(0, prizes.Length);           
            row.randomPrize = prizes[randomPrize]; 
        }
    }

    private IEnumerator SetRowsDesaceleration()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < rowScript.Length; i++)
		{                       
            rowScript[i].desacelerationRate = 0.1f;            			                    
		} 
    }

    private IEnumerator CallSignUpMenu()
    {
        yield return new WaitForSeconds(3);

        if(gameUI.signUpMenuIsOn)
        {
            gameUI.ToggleSignUpMenu(true);
        }
        else
            gameUI.gameStates = GameStates.GameIdle;  
    }

    public void MoveSlots()
    {
        bool logic = true;
        for (int i = 0; i < rows.Length; i++)
        {            
            foreach (Transform child in rows[i].transform)
            {
                child.transform.Translate(Vector3.down * rowScript[i].rowVelocity * Time.smoothDeltaTime);
            }

            if (rowScript[i].rowVelocity >= deadzone)
            {
                if (i == 0) rowScript[i].rowVelocity -= rowScript[i].desacelerationRate;

                else if (rowScript[i - 1].rowVelocity == 0) rowScript[i].rowVelocity -= rowScript[i].desacelerationRate;
            }           
        }

        foreach (RowData script in rowScript)
        {
            logic &= script.rowVelocity <= 0;            
        }

        if (logic) 
        {
            StartCoroutine(ResetSlotMachine());
            slotMachineOn = false;                      
        }
    }

    private void SetRowsVelocity(float value)
    {
        for (int i = 0; i < rowScript.Length; i++)
        {
            rowScript[i].rowVelocity = value;
        }
    }    

    public int GetRowNumber(GameObject prize)
    {
        for (int i = 0; i < rows.Length; i++)
        {            
            if (prize.transform.parent == rows[i].transform)
            {
                return i;
            }
        }
        return 0;
    }

    public void FreezeAllPrizesPosition()
    {
        foreach (RowData row in rowScript)
        {
            foreach (Transform prize in row.transform)
            {
                if(prize.gameObject.GetComponent<Rigidbody>() != null)
                    prize.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }     

    public void SetChosenPrizeAsParent(GameObject row, GameObject chosenPrize)
    {
        List<Transform> prizesList = new List<Transform>();

        foreach (Transform prize in row.transform)
        {
            prizesList.Add(prize);
        }

        row.transform.DetachChildren();

        foreach(Transform prize in prizesList)
        {
            prize.SetParent(chosenPrize.transform, true);
        }

        chosenPrize.transform.SetParent(row.transform, true);
    }

    private IEnumerator ResetSlotMachine()
    {
        List<Transform> prizesList = new List<Transform>();        
        foreach (RowData row in rowScript)
        {
            row.desacelerationRate = 0;
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(CallSignUpMenu());

        settingsButton.SetActive(true);

        if (!isRandom || (isRandom && PrizesMatch()))
            SoundController.PlaySound(SoundController.GameSounds.Fanfare);
        else 
            SoundController.PlaySound(SoundController.GameSounds.SlotFailure);

        Physics.gravity = -Physics.gravity;
        Physics.gravity = new Vector3(Physics.gravity.x, handleGravity, Physics.gravity.z);

        if (desacelerator != null) desacelerator.GetComponent<Collider>().isTrigger = true; 

        foreach (GameObject row in rows)
        {
            GameObject prize = row.transform.GetChild(0).gameObject;
            prize.GetComponent<Rigidbody>().useGravity = false;
            prize.GetComponent<Rigidbody>().constraints -= RigidbodyConstraints.FreezePositionY;
            prize.GetComponent<Collider>().isTrigger = true;
            prizesList.Add(prize.transform);

            foreach (Transform child in prize.transform)
            {
                prizesList.Add(child);                    
            }
            prize.transform.DetachChildren();

            foreach (Transform prizeWithNullParent in prizesList)
            {
                prizeWithNullParent.SetParent(row.transform, true);
            }
                      
            prizesList.Clear();
        }

        DataSavingController savingScript = FindObjectOfType<DataSavingController>();
        if(savingScript.savePrizeName)
        {            
            savingScript.SavePrizeName(prizeChosen.name);
        }

        if (gameUI.signUpComplete) gameUI.signUpComplete = false;

    }

    private void SetPrizesColliderSize()
    {
        foreach (GameObject row in rows)
        {
            foreach (Transform prize in row.transform)
            {
                Vector3 prizeExtent = prize.gameObject.GetComponent<Renderer>().bounds.extents;
                prize.GetComponent<BoxCollider>().size = new Vector3(prizeExtent.x, prizeExtent.y);
                prize.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
            }
        }
    }

    private void SetPrizesQuantity()
    {
        for (int i = 0; i < prizes.Length; i++)
        {
            prizesDictionary.Add(prizes[i], PlayerPrefs.GetInt("Item" + i.ToString()));               
        }
    }

    private void UpdatePrizesQuantity()
    {
        for (int i = 0; i < prizes.Length; i++)
        {            
            prizesDictionary[prizes[i]] = PlayerPrefs.GetInt("Item" + i.ToString());
                       
            //print("Indice = " + i + " Quantidade = " + prizesDictionary[prizes[i]]);
        }
    }   

    void OnDisable()
    {
        updateCalled = false;
        folderFileCount = 0;        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Prize" && slotMachineOn)
        {
            int rowNumber = GetRowNumber(other.gameObject);
            float topmostY = rowScript[rowNumber].GetTopMostElement().transform.localPosition.y;            
            other.transform.localPosition = new Vector3(0, topmostY + heightDifference, 0);

            rowScript[rowNumber].prizesList.Remove(other.gameObject);        
            rowScript[rowNumber].prizesList.AddFirst(other.gameObject);            
        }
    }      
}
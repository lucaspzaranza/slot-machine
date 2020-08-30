using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public enum GameStates
{
    SignUp,
    GameIdle,
    SlotMoving,
    Settings,
}

public class GameUI : MonoBehaviour
{
    #region Variables

    public GameStates gameStates;

    public GameObject slotMachine;
    public GameObject settingsButton;
    public GameObject settingsPanel;
    public GameObject quantityButton;
    public GameObject quantityPanel;
    public GameObject backToSlotButton;
    public GameObject quantityScrollView;
    public GameObject quantityMenuScrollBar;
    public GameObject settingsTitle;
    public GameObject quitButton;
    public GameObject quitConfirmation;
    public GameObject customFolderButton;
    public GameObject slotCanvas;
    public GameObject fileNotFoundWindow;
    public GameObject expiredPanel;
    public GameObject gameIlumination;
    public GameObject piracyPanel;
    public GameObject incorrectPassword;
    public GameObject cancelPasswordButton;

    public Toggle randomToggle;
    public Toggle signUpToggle;

    public GameObject prizeSpritePrefab;
    public GameObject prizeInputPrefab;

    public InputField nameInputField;
    public InputField emailInputField;
    public InputField phoneInputField;
    public InputField passwordInputField;

    public GameObject signUpMenu;

    public GameObject fileExplorer;

    public static InputField currentInput;

    private InputSelector inputSelector;    

    public static SlotMachineController slotMachineController;

    public static DataSavingController dataSavingController;

    public GameObject imagePattern;

    public bool signUpMenuIsOn { get; set; }
    public bool signUpComplete;

    #endregion

    void Start()
    {
        slotMachineController = FindObjectOfType<SlotMachineController>();

        ToggleSignUpMenuActivation();

        signUpComplete = !signUpToggle; 
    }

    public void CallExpiredPanel()
    {
        expiredPanel.SetActive(true);
        gameIlumination.SetActive(false);
        settingsButton.SetActive(false);

        if (signUpMenuIsOn)
        {
            signUpMenuIsOn = false;
            signUpMenu.SetActive(false);
        }            
    }

    public void CallPiracyPanel()
    {
        piracyPanel.SetActive(true);
        gameIlumination.SetActive(false);
        settingsButton.SetActive(false);
    }

    public void ToggleQuantityPassword(bool toggle)
    {
        gameIlumination.SetActive(!toggle);
        backToSlotButton.SetActive(!toggle);
        randomToggle.gameObject.SetActive(!toggle);
        signUpToggle.gameObject.SetActive(!toggle);
        quitButton.SetActive(!toggle);
        quantityButton.SetActive(!toggle);
        customFolderButton.SetActive(!toggle);
        cancelPasswordButton.SetActive(toggle);
        passwordInputField.gameObject.SetActive(toggle);

        if(incorrectPassword.activeInHierarchy) incorrectPassword.SetActive(false);
    }

    public void PasswordVerification()
    {
        if (IsCorret(passwordInputField.text))
        {
            gameIlumination.SetActive(true);
            if (incorrectPassword.activeInHierarchy) incorrectPassword.SetActive(false);
            passwordInputField.gameObject.SetActive(false);
            cancelPasswordButton.SetActive(false);
            ToggleQuantityMenu(true);
            UpdatePrizesQuantityMenu();
        }
        else
        {            
            incorrectPassword.SetActive(true);
        }
    }

    private bool IsCorret(string password)
    {
        passwordInputField.text = string.Empty;
        return password == "touchmitos";
    }   

    public void ToggleSettingsMenu(bool toggle)
    {
        slotMachine.SetActive(!toggle);
        settingsButton.SetActive(!toggle);
        settingsPanel.SetActive(toggle);

        if (slotMachineController.isCustomized)
        {
            quantityButton.SetActive(toggle);
        }        
       
        if (gameStates == GameStates.GameIdle || gameStates == GameStates.SignUp)
        {
            gameStates = GameStates.Settings;
        }
        else if(gameStates == GameStates.Settings)
        {
            if(signUpMenuIsOn && !signUpComplete)            
                gameStates = GameStates.SignUp;                                       
            else
                gameStates = GameStates.GameIdle;
        }

        if (signUpMenuIsOn && !signUpComplete)
            signUpMenu.SetActive(!toggle);                 
    }   

    public void ToggleQuantityMenu(bool toggle)
    {
        if (toggle == false)
        {
            quantityMenuScrollBar.GetComponent<Scrollbar>().value = 0;
            DestroyQuantityMenuPrizes();
        }

        randomToggle.gameObject.SetActive(!toggle);
        quantityButton.SetActive(!toggle);
        backToSlotButton.SetActive(!toggle);
        settingsTitle.SetActive(!toggle);
        quantityPanel.SetActive(toggle);
        quitButton.SetActive(!toggle);
        signUpToggle.gameObject.SetActive(!toggle);
        customFolderButton.SetActive(!toggle);

        if(slotMachine.GetComponent<SlotMachineController>().prizes.Length > 4)
        {
            quantityMenuScrollBar.SetActive(toggle);
        }                
    }

    private void DestroyQuantityMenuPrizes()
    {
        foreach(Transform prize in quantityScrollView.transform)
        {
            Destroy(prize.gameObject);
        }
    }

    public void ToggleRandomGame()
    {        
        slotMachine.GetComponent<SlotMachineController>().isRandom = randomToggle.isOn;
    }

    public void ToggleSignUpMenuActivation()
    {
        signUpMenuIsOn = signUpToggle.isOn;
    }
    
    public void UpdatePrizesQuantityMenu()
    {        
        GameObject newPrize;
        int childIndex = 0;
        int filesInfoIndex = 0;
        float imageSize = imagePattern.GetComponent<Renderer>().bounds.extents.x * 290;       
        
        if (slotMachineController.prizes.Length > 4)
        {
            RectTransform scrollViewRect = quantityScrollView.GetComponent<RectTransform>();           
            scrollViewRect.sizeDelta = new Vector2(imageSize * slotMachineController.prizes.Length, Screen.height);
        }
                           
        slotMachineController.prizeInputTextFieldDictionary = new Dictionary<GameObject,GameObject>();
           
        foreach (GameObject prize in slotMachineController.prizes)
        {
            SpriteRenderer prizeSpriteRenderer = prize.GetComponent<SpriteRenderer>();
            Texture2D newTexture = prizeSpriteRenderer.sprite.texture;               
            Customization.LoadTexture2D(slotMachineController.customFolderDataPath + prize.name + ".png", newTexture);               
                
            filesInfoIndex++;

            Rect textRect = new Rect(0, 0, 350, 450);
            GameObject inputField;

            newPrize = Instantiate(prizeSpritePrefab) as GameObject;
            newPrize.transform.SetParent(quantityScrollView.transform, true);

            childIndex = quantityScrollView.transform.childCount;

            newPrize.name = "Prize Quantity " + childIndex.ToString();

            if (quantityScrollView.transform.childCount > 1)
            {
                Transform lastChild = quantityScrollView.transform.GetChild(childIndex - 2);
                newPrize.transform.SetParent(quantityScrollView.transform, true);
                newPrize.transform.position = new Vector3(lastChild.position.x + 360, lastChild.position.y, lastChild.position.z);
            }
            else
            {
                Camera camera = Camera.main;
                Vector3 screen = new Vector3(0, Screen.height / 2, 10);
                Vector3 newCoordinate = camera.ScreenToWorldPoint(screen);
                newPrize.transform.position = new Vector3(newCoordinate.x + 160, Screen.height / 2, newCoordinate.z);
            }

            inputField = Instantiate(prizeInputPrefab) as GameObject;
            inputField.name = "Input Field " + childIndex.ToString();
            inputField.transform.SetParent(newPrize.transform);
            inputField.GetComponent<InputSelector>().prizeIndex = childIndex - 1;

            slotMachineController.prizeInputTextFieldDictionary.Add(slotMachineController.prizes[childIndex - 1], inputField);

            RectTransform inputRectTransform = inputField.GetComponent<RectTransform>();
            inputRectTransform.localPosition = new Vector3(0, -240, 0);

            newPrize.GetComponent<Image>().sprite = Sprite.Create(newTexture, textRect, new Vector2(0.5f, 0.5f));
        }                             
    }

    public void ToggleQuitConfirmation(bool toggle)
    {
        randomToggle.gameObject.SetActive(!toggle);
        quantityButton.SetActive(!toggle);
        backToSlotButton.SetActive(!toggle);
        settingsTitle.SetActive(!toggle);
        quitConfirmation.SetActive(toggle);
        quitButton.SetActive(!toggle);
        signUpToggle.gameObject.SetActive(!toggle);
        customFolderButton.SetActive(!toggle);
    }

    public void SetNewPrizeQuantity()
    {        
        if(currentInput != null)
        {
            InputField input = currentInput.GetComponent<InputField>();
            int value = int.Parse(input.text);
            int index = currentInput.GetComponent<InputSelector>().prizeIndex;
                      
            PlayerPrefs.SetInt("Item" + index.ToString(), value);

            slotMachineController.prizesDictionary[slotMachineController.prizes[index]] = PlayerPrefs.GetInt("Item" + index.ToString());
        }                          
    }
   
    public void ResetQuantities()
    {
        for (int i = 0; i < slotMachineController.prizes.Length; i++)
        {
            slotMachineController.prizesDictionary[slotMachineController.prizes[i]] = 0; 
            PlayerPrefs.SetInt("Item" + i.ToString(), 0);
        }

        quantityPanel.SetActive(false);
        quantityPanel.SetActive(true);       
    }

    public void UpdateCustomFolderDataPath()
    {
        string newCustomFolderDataPath = slotMachineController.customFolderDataPath;

        slotMachineController.customFolderDataPath = newCustomFolderDataPath;

        PlayerPrefs.SetString("CustomFolder", newCustomFolderDataPath);

        slotMachineController.ResetFrameCount();
    }

    public void ToggleSignUpMenu(bool toggle)
    {
        signUpMenu.SetActive(toggle);
        
        if(gameStates == GameStates.SlotMoving && signUpMenuIsOn) 
        {
            gameStates = GameStates.SignUp;            
        }
        else if(gameStates == GameStates.SignUp && signUpMenuIsOn)
        {
            gameStates = GameStates.GameIdle;           
        }
    }

    public void CompleteSignUp()
    {
        signUpComplete = true;
    }

    public void ToggleFileExplorerMenu(bool toggle)
    {        
        fileExplorer.SetActive(toggle);

        slotCanvas.SetActive(!toggle);
        
        if (toggle == false)
        {
            if (FileExplorer.currentAdress != FileExplorer.rootFolder)
            {
                slotMachineController.customFolderDataPath = FileExplorer.selectedAdress;
                slotMachineController.ResetFrameCount();
            }

            ToggleSettingsMenu(false);
        }        
    }

    public void BackToSettingsMenu()
    {         
         fileExplorer.SetActive(false);
         slotCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }     
}
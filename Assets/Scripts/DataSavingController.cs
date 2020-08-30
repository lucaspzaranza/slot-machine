using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class DataSavingController : MonoBehaviour
{
    #region Variables

    public string dataPath;
    private FileStream textFile;


    public InputField nameInputField;
    public InputField emailInputField;
    public InputField phoneInputField;

    private SlotMachineController slotScript;

    public bool savePrizeName;    
    public Dictionary<string, string> prizesName = new Dictionary<string, string>()
    {
        {"prize 1", "Copo"},
        {"prize 2", "Taça"},
        {"prize 3", "Kit de Maquiagem"}, 
        {"prize 4", "Ingresso"},
        {"prize 5", "Chaveiro"}
    };

    #endregion

    #region Properties

    public string playerName { get; set; }

    public string playerEmail { get; set; }

    public string playerPhone { get; set; }

    private bool nameIsEmpty { get; set; }  

    #endregion

    // Use this for initialization
	void Start () 
    {        
        dataPath = Application.dataPath + "/Cadastros.txt";
        if (Application.platform == RuntimePlatform.WindowsPlayer && !File.Exists(dataPath))
        {
            textFile = File.Create(dataPath);
            textFile.Close();
        }        
	}

    public void UpdatePlayerName()
    {
        playerName = nameInputField.text;
    }

    public void UpdatePlayerEmail()
    {
        playerEmail = emailInputField.text;
    }

    public void UpdatePlayerPhone()
    {
        playerPhone = phoneInputField.text;
    }

    public void SavePlayerData()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (!string.IsNullOrEmpty(playerName))
            {
                File.AppendAllText(dataPath, playerName + ", ");
            }

            if (!string.IsNullOrEmpty(playerEmail))
            {
                File.AppendAllText(dataPath, playerEmail + ", ");
            }

            if (!string.IsNullOrEmpty(playerPhone)) 
            {
                File.AppendAllText(dataPath, playerPhone);
                if (!savePrizeName) File.AppendAllText(dataPath, System.Environment.NewLine);
            }
        }

        nameIsEmpty = string.IsNullOrEmpty(playerName);
    }
       
    public void SavePrizeName(string prize)
    {        
        if (Application.platform == RuntimePlatform.WindowsPlayer && !nameIsEmpty)
            File.AppendAllText(dataPath, ", " + prizesName[prize] + System.Environment.NewLine);
    }

    public void ClearSignUpInputFields()
    {
        nameInputField.text = "";
        emailInputField.text = "";
        phoneInputField.text = "";

        playerName = "";
        playerEmail = "";
        playerPhone = "";
    }
}

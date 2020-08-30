using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CustomFolderInput : MonoBehaviour 
{
    public SlotMachineController slotMachineController;

	void OnEnable()
    {
        GetComponent<InputField>().placeholder.GetComponent<Text>().text = slotMachineController.customFolderDataPath;        
    } 
}

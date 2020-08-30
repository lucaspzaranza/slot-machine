using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputSelector : MonoBehaviour 
{
    public int prizeIndex;

    public SlotMachineController slotMachineController;

    public void SelectNewCurrentInputField()
    {
       GameUI.currentInput = this.GetComponent<InputField>();     
    }       
}

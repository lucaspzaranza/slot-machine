using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputQuantity : MonoBehaviour 
{
    private int index;

    private int frameCount;

    void Start()
    {        
        index = GetComponent<InputSelector>().prizeIndex;        
    }   

    void Update()
    {
        if(gameObject.activeInHierarchy && frameCount == 1)
        {
            UpdateInputFieldText();
        }

        frameCount++;
    }
    
    private void UpdateInputFieldText()
    {
        InputField inputField = GetComponent<InputField>();

        //print("Indice = " + index + " Valor = " + PlayerPrefs.GetInt("Item" + index.ToString()).ToString());

        inputField.text = PlayerPrefs.GetInt("Item" + index.ToString()).ToString();
    }
   
    void OnDisable()
    {
        frameCount = 0;
    }
}
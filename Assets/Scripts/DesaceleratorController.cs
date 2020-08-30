using UnityEngine;
using System.Collections;

public class DesaceleratorController : MonoBehaviour 
{
    public SlotMachineController slotMachineController;
    public GameObject handle;

    private int rowNumber;

    private bool IsThePrizeChosen(GameObject prize)
    {        
        return slotMachineController.prizeChosen != null && prize.name == slotMachineController.prizeChosen.name;
    }

    private bool IsTheRandomlyChosenPrize(GameObject prize, GameObject chosenPrize) 
    {
        return prize.name == chosenPrize.name;
    }
   
    void OnTriggerEnter(Collider other)
    {        
        bool isTheChosenPrize;

        #region Sound Conditional

        if (other.tag == "Prize")
        {
            if (rowNumber == slotMachineController.rows.Length - 1)
            {
                SoundController.PlaySound(SoundController.GameSounds.SlotHit);
            }
        }

        #endregion

        if (slotMachineController.slotMachineOn)
        {
            rowNumber = slotMachineController.GetRowNumber(other.gameObject);

            if (slotMachineController.isRandom)
            {
                isTheChosenPrize = IsTheRandomlyChosenPrize(other.gameObject, slotMachineController.rowScript[rowNumber].randomPrize);
            }                
            else
            {                
                isTheChosenPrize = IsThePrizeChosen(other.gameObject);                
            }
                            
            if (isTheChosenPrize)
            {               
                GameObject chosenPrize = other.gameObject;

                bool logic = chosenPrize.transform.parent.GetComponent<RowData>().rowVelocity >= 0;
                logic &= chosenPrize.transform.parent.GetComponent<RowData>().rowVelocity <= slotMachineController.deadzone;

                if (logic)
                {                                       
                    chosenPrize.transform.parent.GetComponent<RowData>().rowVelocity = 0;

                    GetComponent<Collider>().isTrigger = false;
                    slotMachineController.FreezeAllPrizesPosition();
                    other.isTrigger = false;
                    chosenPrize.GetComponent<Rigidbody>().useGravity = true;
                    chosenPrize.GetComponent<Rigidbody>().constraints -= RigidbodyConstraints.FreezePositionY;

                    GameObject row = other.transform.parent.gameObject;
                    slotMachineController.SetChosenPrizeAsParent(row, chosenPrize);
                    handle.GetComponent<Rigidbody>().useGravity = false;

                    if(Physics.gravity.y > 0)
                    {
                        Physics.gravity = -Physics.gravity;
                        Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
                    }
                        
                    other.gameObject.GetComponent<Rigidbody>().AddForce(0, slotMachineController.bounciness, 0);                    
                } 
            }                                                   
        }            
    }

    void OnCollisionEnter(Collision other)
    {
        bool logic;
                          
        if (slotMachineController.isRandom) 
        {
            rowNumber = slotMachineController.GetRowNumber(other.gameObject);
            GameObject randomlyChosenPrize = slotMachineController.rowScript[rowNumber].randomPrize;
            logic = IsTheRandomlyChosenPrize(other.gameObject, randomlyChosenPrize);
        }            
        else        
            logic = IsThePrizeChosen(other.gameObject);                    

        logic &= other.gameObject.transform.parent.GetComponent<RowData>().rowVelocity == 0;
        logic &= GetComponent<Collider>().isTrigger == false;

        if (logic)
        {
            SoundController.PlaySound(SoundController.GameSounds.PrizeBounce);            
        }
    }
}
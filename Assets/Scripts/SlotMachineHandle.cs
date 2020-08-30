using UnityEngine;
using System.Collections;

public class SlotMachineHandle : MonoBehaviour 
{
    public Renderer slotMachineRenderer;    
    public SlotMachineController slotMachineController;
    public float border;
    public float velocityTrigger;

    private Vector3 initialPosition;

    public GameUI gameUI;

    void Start()
    {
        initialPosition = transform.position;
    }

    void OnMouseDrag()
    {       
        bool logic = !slotMachineController.slotMachineOn && !GetComponent<Rigidbody>().useGravity;
        logic &= !slotMachineController.isExpired;
        //logic &= slotMachineController.isOurComputer;
          
        if (gameUI.signUpMenuIsOn)
        {
            logic &= !gameUI.signUpMenu.activeInHierarchy;
            logic &= gameUI.gameStates == GameStates.GameIdle;
        }

        if (logic)
        {
            Vector3 mouseCoordinates = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mouseY = mouseCoordinates.y;
            mouseY = Mathf.Clamp(mouseCoordinates.y, -slotMachineRenderer.bounds.extents.y + border, slotMachineRenderer.bounds.extents.y - border /*+ 0.2f*/);
            transform.position = new Vector3(transform.position.x, mouseY, transform.position.z);
        }       
    }

    void OnMouseUp()
    {
        bool changedPosition = transform.position.y < initialPosition.y;

        if (!slotMachineController.slotMachineOn && !GetComponent<Rigidbody>().useGravity && changedPosition)
        {
            GetComponent<Rigidbody>().useGravity = true;
            SoundController.PlaySound(SoundController.GameSounds.HandleRelease);
        }            
    }
    
    void OnCollisionEnter(Collision other)
    {        
        bool logic = !slotMachineController.slotMachineOn;
        logic &= GetComponent<Rigidbody>().useGravity;
        logic &= GetComponent<Rigidbody>().velocity.y <= velocityTrigger;

        if (logic)
        {
            slotMachineController.settingsButton.SetActive(false);
            slotMachineController.ActivateSlotMachine();
        }        
    }

    void OnCollisionStay(Collision other)
    {
        bool logic = !slotMachineController.slotMachineOn;
        logic &= !GetComponent<Rigidbody>().useGravity;
        logic &= GetComponent<Rigidbody>().velocity.y == 0;

        if (!logic)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); 
        }
    }
}
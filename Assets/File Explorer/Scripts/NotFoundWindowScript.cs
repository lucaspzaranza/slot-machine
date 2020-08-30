using UnityEngine;
using System.Collections;

public class NotFoundWindowScript : MonoBehaviour
{
    private int frameCount;
    public int framesToFadeWindow;	
	
	// Update is called once per frame
	void Update () 
    {
        frameCount++;

        if(frameCount == framesToFadeWindow)
        {
            frameCount = 0;
            gameObject.SetActive(false);
        }
	}
}
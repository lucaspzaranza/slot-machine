using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RowData : MonoBehaviour 
{
    //[Range(0f, 20f)] Transforma em slide! xD
    public float rowVelocity;
    public float desacelerationRate {get; set;}
    public LinkedList<GameObject> prizesList;
    public GameObject randomPrize;

    void Start()
    {       
        prizesList = new LinkedList<GameObject>();        

        StartCoroutine(GetPrizes());        
    }

    private IEnumerator GetPrizes()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (Transform prize in transform)
        {
            prizesList.AddLast(prize.gameObject);
        }
    }

    public GameObject GetTopMostElement()
    {
        LinkedListNode<GameObject> topmost = prizesList.First;
        LinkedListNode<GameObject> next = prizesList.First.Next;
       
        for (int i = 1; i < prizesList.Count; i++) 
        {            
            if (topmost.Value.transform.position.y < next.Value.transform.position.y)
            {
                topmost = next;
                next = next.Next;
            }
        }       
               
        return topmost.Value;
    }
}

    
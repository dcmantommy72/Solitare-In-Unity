using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{

    //private bool isDropped = false;



    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop");
        
        //Get GameObject that is currently being dragged
        //eventData.pointerDrag is the item being dragged
        if (eventData.pointerDrag != null)
        {
            // if drop 
            //isDropped = true;
            //set the anchorpos of the dragged item, to the pos of the drop item..
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }


    // public bool wasDropped()
    // {
    //     if (isDropped == true)
    //     {
    //         //item is droppable so return true
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }



}

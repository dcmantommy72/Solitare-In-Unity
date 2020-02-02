using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    //[SerializeField] private Canvas canvas;
    public GameObject testDrag; //The Drag Into Object
    //private Vector3 ogCardPos;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;
    private ItemSlot itemSlot;
    private Vector2 slotPos;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPos = GetComponent<RectTransform>().anchoredPosition; //Set OG Pos
        // Get the dragspot pos
        slotPos = testDrag.GetComponent<RectTransform>().anchoredPosition;
    }

    //OnBeginDrag: Called when mouse down on object and mouse moves
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    //OnDrag: Gets called each frame we are currently holding down button
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
        rectTransform.anchoredPosition += eventData.delta; // moves object bein dragged

    }



    //OnEndDrag: Called when object is let go
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        //If item was NOT dropped into ItemSlot then reset items pos
         // Go back to OG pos
        if (rectTransform.anchoredPosition != slotPos)
            {
                //Then item was not dropped, reset pos
                rectTransform.anchoredPosition = originalPos;
            }
    }

    //OnPointerDown: Called when mouse pointer is pressed down on object
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    //OnDrop: Called when object is Dropped Into this
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop DRAG AND DROP SCRIPT");
    }







}

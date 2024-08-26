using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour 
{   
    public GameObject canvas;
    public GameObject dropZone;
    public GameObject passTurnButtonGameObject;
    public Button passTurnButton;


    private bool isDragging = false;
    private GameObject startParent;
    private Vector2 startPosition;
    public GameObject drop_Zone;
    public static bool canPlay = false;
    public static int currentPlayer = 2;
    private bool isOverDropZone;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        drop_Zone = GameObject.Find("RowMP1");

        passTurnButtonGameObject = GameObject.Find("PassTurnButton");
        passTurnButton = passTurnButtonGameObject.GetComponent<Button>();
        // Desactivar el botón de pasar turno al inicio
        drop_Zone = null;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        drop_Zone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        drop_Zone = null;
    }

    public void StartDrag()
    {
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
    }

    public void EndDrag()
    {
        isDragging = false;
        if(isOverDropZone)
        {   
            CardDisplay draggableObject = GetComponent<CardDisplay>();
            if (draggableObject!= null)
            {   
                bool valid = false;
                foreach (string validZone in draggableObject.validZonesTag)
                {
                    if(drop_Zone.tag == validZone)
                    {   
                        valid = true;
                        transform.SetParent(drop_Zone.transform, false);
                        draggableObject.card.ActivateEffect(draggableObject, drop_Zone);
                        
                        EventTrigger eventTrigger = GetComponent<EventTrigger>();
                        if (eventTrigger!= null && draggableObject)
                        {
                            eventTrigger.enabled = false;
                        }

                        if(currentPlayer == 1 && !TurnController.endRound)
                        {
                            GameManager.Instance.handP2.SetActive(false);
                                    
                        }
                        else if (currentPlayer == 2 && !TurnController.endRound)
                        {
                            GameManager.Instance.handP1.SetActive(false);
                               
                        }
                        currentPlayer = (currentPlayer % 2) + 1;
                        TurnController.hasPlayerPlayed = true;
                        break;

                    }
                }
                if (!valid)
                {
                    transform.position = startPosition;
                    transform.SetParent(startParent.transform, false);
                }    
            } 
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    void Update()
    {   
        if(isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(canvas.transform, true);
        }
    }

}
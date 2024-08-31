using UnityEngine;
using UnityEngine.EventSystems;
using GwentPlus;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private GameObject draggedObject; // Almacenar el objeto que se esta arrastrando
    private RectTransform rectTransform; // Referencia al componente RectTransform para manipular la posición y tamaño del objeto en la UI
    private CanvasGroup canvasGroup; // Referencia al componente CanvasGroup para controlar la visibilidad y la interacción del objeto
    private Vector3 startPosition; // Almacenar la posicion inicial del objeto para poder evertir el arrastre si es necesario 
    private string currentCardType; // Almacena el tipo de carta que se está arrastrando
    public static bool isDrag = false;
    private static CardDisplay currentDraggingCard;

    
    public static CardDisplay GetCurrentDraggingCard()
    {
        return currentDraggingCard;
    }

    // Se ejecuta al iniciar el objeto, antes del primer frame
    private void Awake()
    {
        // Obtiene el componente RectTransform del objeto para manipular su posición y tamaño
        rectTransform = GetComponent<RectTransform>();
        // Obtiene el componente CanvasGroup para controlar la visibilidad y la interacción
        canvasGroup = GetComponent<CanvasGroup>();


    }

    // Se ejecuta en cada frame
    private void Update()
    {
        // Inicializa la búsqueda desde el objeto actual
        Transform currentTransform = transform;

        // Busca hacia arriba en la jerarquía de objetos hasta encontrar uno con el tag "PlayerHand"
        while (currentTransform != null)
        {
            // Si encuentra un objeto con el tag "PlayerHand", habilita el componente DragDrop
            if (currentTransform.tag == "PlayerHand")
            {
                enabled = true;
                return; // Termina la búsqueda
            }
            // Sube un nivel en la jerarquía para buscar en el objeto padre
            currentTransform = currentTransform.parent;
        }
        // Si no se encuentra, deshabilita el componente DragDrop
        enabled = false;
    }

    // Se llama al inicio del arrastre
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        // Evita el arrastre si el componente está desactivado
        if (!enabled) return;

        // Asigna el objeto que está siendo arrastrado
        draggedObject = eventData.pointerDrag; 
        
        currentDraggingCard = draggedObject.GetComponent<CardDisplay>();
        
        // Actualiza el tipo de carta que se está arrastrando
        CardDisplay cardDisplay = draggedObject.GetComponent<CardDisplay>();
        currentCardType = cardDisplay.card.Type.ToString();

        // Guarda la posicion inicial del objeto al comenzar el arrastre 
        startPosition = rectTransform.anchoredPosition;

        // Reduce la opacidad para indicar que el objeto está siendo arrastrado
        canvasGroup.alpha = .6f;

        // Permite que otros objetos interactúen con el objeto mientras se arrastra
        canvasGroup.blocksRaycasts = false;

    }

    // Se llama durante el arrastre
    public void OnDrag(PointerEventData eventData)
    {
        // Evita el arrastre si el componente está desactivado
        if (!enabled) return;
    
        Vector3 mousePosition = Input.mousePosition;
        rectTransform.position = mousePosition;        
    }

    // Se llama al finalizar el arrastre
    public void OnEndDrag(PointerEventData eventData)
    {
        // Evita el arrastre si el componente está desactivado
        if (!enabled) return;

        // Restaura la opacidad original
        canvasGroup.alpha = 1f;
        // Bloquea las interacciones con otros objetos
        canvasGroup.blocksRaycasts = true;

        // // Obtener el CardDisplay del objeto arrastrado
        CardDisplay cardDisplay = draggedObject.GetComponent<CardDisplay>();
        

        if (TriggerLogger.isColling)
        {
            draggedObject.transform.SetParent(TriggerLogger.otherCard.transform.parent);
            TriggerLogger.isColling = false;
            TriggerLogger.otherCard = null;
            GameManager.Instance.ActualizeContext();
            cardDisplay.card.ActivateEffects();
            GameManager.Instance.ActualiceVisual();
            GameManager.Instance.StartTurn();
            EndRound.counter = 0;

        }
        //Verificar si el objeto fue soltado en una zona valida
        else if (!IsDroppedInValidZone(eventData, cardDisplay.card.Range))
        {
            rectTransform.anchoredPosition = startPosition;
        }
        else 
        {
            // Si la zona es válida, cambia el padre y ajusta la posición del objeto arrastrado
            ValidZone validZone = eventData.pointerCurrentRaycast.gameObject.GetComponent<ValidZone>();
            if (validZone != null)
            {
                validZone.PlaceObject(rectTransform);
                GameManager.Instance.ActualizeContext();
                cardDisplay.card.ActivateEffects();
                GameManager.Instance.ActualiceVisual();
                GameManager.Instance.StartTurn();

                EndRound.counter = 0;
            }
        }

        isDrag = false;

    }

    // Método para verificar si el objeto fue soltado en una zona válida
    private bool IsDroppedInValidZone(PointerEventData eventData, Range[] ranges)
    {
        // Obtiene el objeto sobre el cual se soltó el objeto arrastrado
        GameObject droppedOnObject = eventData.pointerCurrentRaycast.gameObject;

        // Intenta obtener el componente ValidZone del objeto sobre el cual se soltó el objeto arrastrado
        ValidZone validZone = droppedOnObject.GetComponent<ValidZone>();

        if (currentCardType == "Oro" || currentCardType == "Plata")
        {
            // Si el objeto sobre el cual se soltó el objeto arrastrado tiene el componente ValidZone y ranges no es null
            if (validZone != null && ranges != null)
            {
                foreach (Range range in ranges)
                {
                    if (range.ToString() == validZone.zoneType && validZone.owner == GameManager.Instance.currentPlayer)
                    {
                        return true;
                    }
                }
            }
        }
        else if (currentCardType == "Clima" && validZone != null)
        {
            if (validZone.zoneType == "Clima")
            {
                TriggerLogger triggerLogger = draggedObject.GetComponent<TriggerLogger>();
                triggerLogger._enabled = false;
                return true;
            }
        }
        else if (currentCardType == "Aumento" && validZone != null)
        {
            if (validZone.zoneType != "Clima")
            {
                return true;
            }
        }
        else if (currentCardType == "Despeje" || currentCardType == "Senuelo")
        {
            if (TriggerLogger.isColling)
            {
                return true;
            }
            
            return false;
        }
        
        return false;
    }
}

using UnityEngine.EventSystems;
using UnityEngine;

public class ValidZone : MonoBehaviour
{
    public int owner;
    public string zoneType; 

    // Método para cambiar el padre y ajustar la posición del objeto arrastrado, solo se llama si la zona es válida
    public void PlaceObject(RectTransform draggedRect)
    {
        // Cambia el padre del objeto arrastrado al contenedor para que se integre en el HorizontalLayoutGroup
        draggedRect.SetParent(transform);
        // Ajusta la posición del objeto arrastrado para que se coloque correctamente dentro del nuevo padre
        draggedRect.anchoredPosition = Vector2.zero; // Esto asegura que el objeto arrastrado comience desde el borde izquierdo del contenedor
    }
}
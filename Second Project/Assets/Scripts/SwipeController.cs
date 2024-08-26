using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GwentPlus;

public class swipeController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 targetPos;
    Vector3 targetPosButton;

    [SerializeField] Vector3 pageStep;
    [SerializeField] Vector3 pageStepButton;

    [SerializeField] RectTransform factionPagesRect;
    [SerializeField] RectTransform factionButtonPagesRect;

    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    float dragThreshould;

    public void Awake()
    {
        currentPage = 1;
        targetPos = factionPagesRect.localPosition;
        targetPosButton = factionButtonPagesRect.localPosition;
        dragThreshould = Screen.width / 15;
    }
    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            targetPosButton += pageStepButton;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            targetPosButton -= pageStepButton;
            MovePage();
        }
    }

    void MovePage()
    {
        factionPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        factionButtonPagesRect.LeanMoveLocal(targetPosButton, tweenTime).setEase(tweenType);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) >  dragThreshould)
        {
            if (eventData.position.x > eventData.pressPosition.x) Previous();
            else Next();
        }
        else 
        {
            MovePage();
        }
    }
}

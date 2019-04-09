using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FieldType
{
    SELF_HAND,
    ENEMY_HAND, 
    DECK_FIELD,
    DISCARDED_FIELD
}

public class DropPlace : MonoBehaviour, IDropHandler
{
    public FieldType Type;
    public GameManagerSrc GameManager;
    void Awake()
    {
        GameManager = FindObjectOfType<GameManagerSrc>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (Type != FieldType.DISCARDED_FIELD || !GameManager.IsPlayerTurn)
            return;
        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();

        if (card)
        {
            card.DefaultParrent = transform;
            if (Type == FieldType.DISCARDED_FIELD)
            {
                if(this.gameObject.transform.childCount != 0)
                    Destroy(this.gameObject.transform.GetChild(0).gameObject);
            }
        }
            
    }
}
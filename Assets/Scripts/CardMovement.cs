using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParrent;
    public bool isDragable;
    public GameManagerSrc GameManager;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        GameManager = GameManagerSrc.Instance;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = this.transform.position - MainCamera.ScreenToWorldPoint(eventData.position);
        DefaultParrent = transform.parent;
        isDragable = DefaultParrent.GetComponent<DropPlace>()?.Type == FieldType.SELF_HAND && GameManager.IsPlayerTurn;
        if (!isDragable)
            return;
        transform.SetParent(DefaultParrent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragable)
            return;
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        this.transform.position = newPos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragable)
            return;
        transform.SetParent(DefaultParrent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        
    }

    public void SetAsDiscarded()
    {
        if (GameManager.DiscerdedPlace.gameObject.transform.childCount != 0)
            Destroy(GameManager.DiscerdedPlace.gameObject.transform.GetChild(0).gameObject);
        transform.SetParent(GameManager.DiscerdedPlace);
        this.GetComponent<CardControl>().Info.ShowCardInfo();

    }

    public void Discard()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(GameManager.DiscerdedPlace.position, .5f);
    }
}

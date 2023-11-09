using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Vector3 locationAfterDrag;
    public int Id {get; private set;}
    private Texture2D _image;
    private Image _img;
    public Utilities.Direction Direction {get; private set;}

    public void SetCard(int cardId)
    {
        Debug.Log("card id: " + cardId);
        SetCard(Cards.GetInstance().GetCardById(cardId));
    }

    public void SetCard(Cards.CardInfo cardInfo)
    {
        Id = cardInfo.cardId;
        _image = cardInfo.image;
        _img = GetComponent<Image>();
        _img.sprite = Sprite.Create(_image, new Rect(0, 0, _image.width, _image.height), Vector2.zero);
        Direction = cardInfo.direction;
    }

    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        locationAfterDrag = transform.position;
        transform.SetAsLastSibling();
        _img.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        transform.position = locationAfterDrag;
        _img.raycastTarget = true;
    }
    
    void Onclicked()
    {
        
    }
}

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSpacesGridController : MonoBehaviour, IDropHandler
{
    public GameObject emptyCardSpacePrefab;

    [SerializeField] private RoundController roundController;

    private List<GameObject> cardSpacesList;

    private List<GameObject> cards;

    private void Start() 
    {
        cardSpacesList = new List<GameObject>();
        for (int i = 0; i < StaticVariables.AMOUNT_OF_PLAY_CARDS; i++)
        {
            GameObject current = Instantiate(emptyCardSpacePrefab);
            current.transform.SetParent(gameObject.transform);
            current.transform.SetSiblingIndex(i);
            cardSpacesList.Add(current);
        }
        cards = new List<GameObject>();
    }

    public void ClearCards() 
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }
        cards.RemoveAll(x => true);
        cardSpacesList.ForEach(x => x.gameObject.SetActive(true));
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        CardScript card = droppedObject.GetComponent<CardScript>();
        card.transform.SetParent(cardSpacesList[cards.Count].gameObject.transform);
        card.locationAfterDrag = cardSpacesList[cards.Count].gameObject.transform.position;
        cards.Add(droppedObject);
        roundController.PickCards(cards.Select(card => card.GetComponent<CardScript>()).ToList());
    }
}

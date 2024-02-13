using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardTypes;
    [SerializeField] private GameObject handGrid;
    [SerializeField] private CardSpacesGridController cardSpacesGridController;

    public void FillHand(int amount) {
        foreach (Transform card in handGrid.transform) 
        {
            Destroy(card.gameObject);
        }

        List<GameObject> cards = GetCards(amount);
        foreach (GameObject card in cards) {
            GameObject cardObject = Instantiate(card, Vector3.zero, Quaternion.identity);
            cardObject.transform.SetParent(handGrid.transform);
        }
    }

    public void GetCards(List<int> cardIds)
    {
        foreach (Transform card in handGrid.transform) 
        {
            Destroy(card.gameObject);
        }
        foreach (int cardId in cardIds) {
            GameObject cardObject = Instantiate(cardTypes.First(), Vector3.zero, Quaternion.identity);
            cardObject.GetComponent<CardScript>().SetCard(cardId);
            cardObject.transform.SetParent(handGrid.transform);
        }
    }

    public List<GameObject> GetCards(int amount)
    {
        List<GameObject> cards = new();
        for (int i = 0; i < amount; i++) {
            cards.Add(GetRandomCard());
        }
        return cards;
    }

    public List<int> GetCardIds(int amount)
    {
        return Cards.GetCardIds(amount);
    }

    private GameObject GetRandomCard()
    {
        int random = Random.Range(0, 4);
        return cardTypes[random];
    }

    public void ClearCards()
    {
        System.Array.ForEach(handGrid.GetComponentsInChildren<CardScript>(), component => Destroy(component.gameObject));
        cardSpacesGridController.ClearCards();
    }
}

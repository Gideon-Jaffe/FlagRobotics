using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cards : MonoBehaviour
{
    [SerializeField] private List<Texture2D> images;

    private static Cards instance;
    
    public static Cards GetInstance()
    {
        if (instance == null)
        {
            instance = new Cards();
        }
        return instance;
    }

    void Start()
    {
        instance = this;
    }

    public CardInfo GetCardById(int id)
    {
        return id switch
        {
            0 => new CardInfo {cardId = id, direction = Utilities.Direction.Forward, image = images[id]},
            1 => new CardInfo {cardId = id, direction = Utilities.Direction.Back, image = images[id]},
            2 => new CardInfo {cardId = id, direction = Utilities.Direction.Right, image = images[id]},
            3 => new CardInfo {cardId = id, direction = Utilities.Direction.Left, image = images[id]},
            _ => throw new System.NotImplementedException(),
        };
    }

    public static List<int> GetCardIds(int amount)
    {
        List<int> cards = new();
        for (int i = 0; i < amount; i++) {
            cards.Add(Random.Range(0, 4));
        }
        return cards;
    }

    public class CardInfo
    {
        public int cardId;
        public string name;
        public Utilities.Direction direction;
        public Texture2D image;
    }
}

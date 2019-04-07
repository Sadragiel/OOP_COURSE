using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string Name;
    public Sprite Logo;
    public Card(string Name, string LogoPath)
    {
        this.Name = Name;
        this.Logo = Resources.Load<Sprite>(LogoPath);
    }
}

public static class CardStorage
{
    public static List<Card> AllCards = new List<Card>();
}

public class CardManager : MonoBehaviour
{
    public void Awake()
    {
        CardStorage.AllCards.Add(new Card("Jeanne", "Sprites/Jeanne"));
        CardStorage.AllCards.Add(new Card("Atalanta", "Sprites/Atalanta"));
        CardStorage.AllCards.Add(new Card("Kiyohime", "Sprites/Kiyohime"));
        CardStorage.AllCards.Add(new Card("Mash", "Sprites/Mash"));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardEffects;
/*
 Класс для создания Карт
 Здесь будут создаваться карты и колода
 Карты Взрыва и Обезвреживания добавляться будут тут же
     */



public class Card : ICloneable
{
    public string Name;
    public Sprite Logo;

    //Bridge+Comand
    public Effect Effect;

    public Card() {}
    public Card(string Name, string LogoPath, Effect Effect)
    {
        this.Name = Name;
        this.Logo = Resources.Load<Sprite>(LogoPath);
        this.Effect = Effect;
    }

    public object Clone()
    {
        object clone = null;
        try
        {
            clone = this.MemberwiseClone();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return clone;
    }
}

public class CardCreator
{
    Card template;
    public CardCreator()
    {
        template = new Card();
    }
    public Card Create(string Name, string LogoPath, Effect Effect)
    {
        Card newCard = template.Clone() as Card;
        newCard.Name = Name;
        newCard.Logo = Resources.Load<Sprite>(LogoPath);
        newCard.Effect = Effect;
        return newCard;
    }
}


public static class CardStorage
{
    public static List<Card> AllCards = new List<Card>();
}

public class CardManager : MonoBehaviour
{
    CardCreator Creator;
    public void Awake()
    {
        Creator = new CardCreator();
        CardStorage.AllCards.Add(Creator.Create("Jeanne", "Sprites/Jeanne", new Attack()));
        CardStorage.AllCards.Add(Creator.Create("Atalanta", "Sprites/Atalanta", new SkipTurn()));
        CardStorage.AllCards.Add(Creator.Create("Kiyohime", "Sprites/Kiyohime", new Shuffle()));
        CardStorage.AllCards.Add(Creator.Create("Mash", "Sprites/Mash", new Check()));
    }
}

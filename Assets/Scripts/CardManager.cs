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
    public bool isPlayable;

    //Comand
    public Effect Effect;
    public PreEffect PreEffect;

    public Card() {}
    public Card(string Name, string LogoPath, bool isPlayable, Effect Effect, PreEffect PreEffect = null)
    {
        this.Name = Name;
        this.Logo = Resources.Load<Sprite>(LogoPath);
        this.isPlayable = isPlayable;
        this.Effect = Effect;
        this.PreEffect = PreEffect;
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
    public Card Create(string Name, string LogoPath, bool isPlayable, Effect Effect, PreEffect PreEffect = null)
    {
        Card newCard = template.Clone() as Card;
        newCard.Name = Name;
        newCard.Logo = Resources.Load<Sprite>(LogoPath);
        newCard.isPlayable = isPlayable;
        newCard.Effect = Effect;
        newCard.PreEffect = PreEffect;
        return newCard;
    }
}


public class CardManager : MonoBehaviour
{
}

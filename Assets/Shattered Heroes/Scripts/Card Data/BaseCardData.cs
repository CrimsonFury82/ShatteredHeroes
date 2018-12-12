using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BaseCardData : ScriptableObject
{
    public string cardName, ability1, ability2;
    
    public int hp;

    public Sprite artSprite;

    public AudioClip audio1;

    public virtual void BaseCardUpdate(Text cardNameText, Text ability1Text, Text ability2Text, Text hpText, Image artImage)
    {
        cardNameText.text = cardName;
        ability1Text.text = ability1;
        ability2Text.text = ability2;
        hpText.text = hp.ToString();
        artImage.sprite = artSprite;
    }
}
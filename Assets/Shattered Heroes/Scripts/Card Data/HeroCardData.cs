using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Hero", menuName = "Card/Hero")]
public class HeroCardData : BaseCardData
{
    public Sprite artSprite2, artSprite3, artSprite4;
    public AudioClip audio2, audio3, audio4;

    public void HeroArt2(HeroCardPrefab hero)
    {
        hero.artImage2.sprite = artSprite2;
    }
}
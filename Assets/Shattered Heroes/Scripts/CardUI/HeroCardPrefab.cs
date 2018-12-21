using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardPrefab : BaseCardPrefab
{
    public HeroCardData heroCardData;

    void Start()
    {
        UIHeroCard(cardNameText, ability1Text, ability2Text, hpText, artImage);
    }

    public void UIHeroCard(Text cardName, Text ability1, Text ability2, Text def, Image artImage) //pases arguments to BasdeCardUpate()
    {
        heroCardData.BaseCardUpdate(cardNameText, ability1Text, ability2Text, hpText, artImage);
    }
}
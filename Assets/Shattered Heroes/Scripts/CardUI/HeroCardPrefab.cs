using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardPrefab : BaseCardPrefab
{
    public HeroCardData heroCardData;

    public Image artImage2, artImage3, artImage4;

    void Start()
    {
        UIHeroCard(cardNameText, ability1Text, ability2Text, hpText, artImage);
    }

    public void UIHeroCard(Text cardName, Text ability1, Text ability2, Text def, Image artImage)
    {
        heroCardData.BaseCardUpdate(cardNameText, ability1Text, ability2Text, hpText, artImage);
    }
}
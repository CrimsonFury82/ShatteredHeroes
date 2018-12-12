using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardUI : BaseCardUI
{
    public HeroCardData heroCardData;

    void Start()
    {
        //HeroCardData card = Instantiate(heroCardData);
        UIHeroCard(cardNameText, ability1Text, ability2Text, hpText, artImage);
    }

    public void UIHeroCard(Text cardName, Text ability1, Text ability2, Text def, Image artImage)
    {
       heroCardData.BaseCardUpdate(cardNameText, ability1Text, ability2Text, hpText, artImage);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public abstract class BaseCardPrefab : MonoBehaviour
{
    BaseCardData baseCardData;
    
    public Image artImage;

    public Text cardNameText, ability1Text, ability2Text, hpText;

    void Start() //pases arguments to BasdeCardUpate()
    {
        baseCardData.BaseCardUpdate(cardNameText, ability1Text, ability2Text, hpText, artImage); //updates prefab UI text with values from scriptable object
    }
}
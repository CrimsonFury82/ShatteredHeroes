using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Keyword Abilities", menuName = "New Keyword Abilities")]
public class CardKeywords : ScriptableObject
{
    public void Heal() // Run some keyword functionality
    {
        Debug.Log("I healed");
    }

    public void Keyword2() // Run some keyword functionality
    {
        Debug.Log("keyword 2");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Keyword Abilities", menuName = "New Keyword Abilities")]
public class CardKeywords : ScriptableObject
{
    public void Keyword1() // Run some keyword functionality
    {
        Debug.Log("keyword 1");
    }

    public void Keyword2() // Run some keyword functionality
    {
        Debug.Log("keyword 2");
    }
}

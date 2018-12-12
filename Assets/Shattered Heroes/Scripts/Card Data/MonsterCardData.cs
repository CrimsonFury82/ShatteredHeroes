using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "New Monster", menuName = "Card/Monster")]
public class MonsterCardData : BaseCardData
{
    public int attack, cooldown;
}
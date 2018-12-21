using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MonsterCardPrefab : BaseCardPrefab
{
    public MonsterCardData monsterCardData;
    
    public Text attackText, cooldownText;

    public GameController gameController;

    public Button button;

    public AudioSource audioSource;

    public AudioClip cardPlayedSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameController = (GameController)FindObjectOfType(typeof(GameController)); //finds the gamecontroller
        UIMonsterCard(attackText, cooldownText); //calls the UI function to update card text
    }

    void UIMonsterCard(Text attack, Text cooldown) 
    {
        monsterCardData.BaseCardUpdate(cardNameText, ability1Text, ability2Text, hpText, artImage);  //pases arguments to BasdeCardUpate()
        attack.text = monsterCardData.attack.ToString(); //updates prefab with values from scriptable object
        cooldown.text = monsterCardData.cooldown.ToString(); //updates prefab with values from scriptable object
    }

    public void PlayClickedCard() //plays the clicked card from your hand area to the battlezone
    {
        gameController.PlayCard(this.gameObject, monsterCardData, button, audioSource);
    }

    public void PlaySound() //plays audio clip once
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = monsterCardData.audio1;
        audioSource.PlayOneShot(attackSound);
    }
}
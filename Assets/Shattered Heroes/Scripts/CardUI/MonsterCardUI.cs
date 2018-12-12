using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MonsterCardUI : BaseCardUI
{
    public MonsterCardData monsterCardData;
    
    public Text attackText, cooldownText;

    public GameController gameController;

    public Button button;

    AudioSource audioSource;

    public AudioClip cardSound;

    GameObject card; 

    void Start()
    {
        card = this.gameObject;
        audioSource = GetComponent<AudioSource>();
        gameController = (GameController)FindObjectOfType(typeof(GameController)); //finds the gamecontroller
        UIMonsterCard(cardNameText, ability1Text, ability2Text, hpText, artImage, attackText, cooldownText); //calls the UI function to update card text
    }

    void UIMonsterCard(Text cardName, Text ability1, Text ability2, Text hp, Image artImage, Text attack, Text cooldown)
    {
        monsterCardData.BaseCardUpdate(cardNameText, ability1Text, ability2Text, hpText, artImage); //updates prefab with values from scriptable object
        attack.text = monsterCardData.attack.ToString(); //updates prefab with values from scriptable object
        cooldown.text = monsterCardData.cooldown.ToString(); //updates prefab with values from scriptable object
    }

    public void Play()
    {
        PlayClickedCard(card);
    }

    public void PlayClickedCard(GameObject tempCard) //plays the clicked card from your hand area to the battlezone
    {
        audioSource.PlayOneShot(cardSound);
        gameController.PlayCard(this.gameObject, monsterCardData, button, audioSource);
    }
}
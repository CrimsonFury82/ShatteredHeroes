using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public enum turn {Player1, Player2}; //List of states
    public enum phase {MainPhase, CombatPhase}; //List of states

    turn turnState; //State for current player turn

    public phase turnPhase; //State for current game phase

    int p1HP = 99, p2HP = 99, turnsRemaining = 21, defenderHeroHP;

    public GameObject p1HandObject, GameOverObject, P1Hero, P2Hero;

    GameObject attackTarget;
    
    public Text turnText, p1DeckText, p2DeckText, p1HandText, p2HandText, p1HealthText, p2HealthText, gameoverText; //UI text

    public Transform p1BattleTransform, p2BattleTransform, p1HandTransform, p2HandTransform, handTransform, battleTransform; //Board areas for each players cards

    public List<MonsterCardData> p1Deck, p2Deck, deckList, p1Hand, p2Hand, handList; //lists for the carddata of each decks and hand

    public List<GameObject> p1LiveMonsters, p2LiveMonsters, deadMonsters, attackers, defenders; //lists for the card prefabs on the board and dead cards

    public MonsterCardUI monsterCardTemplate; //card prefab

    MonsterCardUI monsterCardUI; //script

    public AnimationController animationController;

    MonsterCardData topDeckCard;
            
    public void Turns() //function for turn states
    {   switch (turnState)
        {
            case turn.Player1:
                break;
            case turn.Player2:
                break;
            default:
                print("Default Turn triggered");
                break;
        }
    }

    public void Phases() //function for phase states
    {
        switch (turnPhase)
        {
            case phase.MainPhase:
                break;
            case phase.CombatPhase:
                break;
            default:
                print("Default Phase triggered");
                break;
        }
    }

    void Start()
    {
        BeginGame();
    }

    void BeginGame() //function for start of game.
    {
        HeroHPUIUpdate(); //updates UI text
        DealCard(p1Deck[0], p1Deck, p1HandTransform, p1Hand); //deals card to P1
        DealCard(p1Deck[0], p1Deck, p1HandTransform, p1Hand); //deals card to P1
        DealCard(p2Deck[0], p2Deck, p2HandTransform, p2Hand); //deals card to P2
        DealCard(p2Deck[0], p2Deck, p2HandTransform, p2Hand); //deals card to P2
        turnState = turn.Player2;
        TurnSwap();
    }

    public void DeckUIUpdate() //updates deck & hand count UI text
    {
        p1DeckText.text = p1Deck.Count.ToString();
        p2DeckText.text = p2Deck.Count.ToString();
        p1HandText.text = p1Hand.Count.ToString();
        p2HandText.text = p2Hand.Count.ToString();
    }

    void HeroHPUIUpdate() //updates Hero HP UI text
    {
        p1HealthText.text = p1HP.ToString();
        p2HealthText.text = p2HP.ToString();
    }

    void PhaseUIUpdate() //updates current phase UI text
    {
        turnText.text = turnState + " " + turnPhase + ": Turns remaining " + turnsRemaining;
    }

    void TurnCountUpdate() //increments turn count
    {
        turnsRemaining--;
        PhaseUIUpdate();
    }

    public void TurnSwap() //function for Start of turn
    {
        turnPhase = phase.MainPhase;

        if (turnState == turn.Player1) //checks if currently Player 1's turn
        {
            turnState = turn.Player2;
            attackTarget = P1Hero;
            deckList = p2Deck;
            handTransform = p2HandTransform;
            handList = p2Hand;
            attackers = p2LiveMonsters;
            defenders = p1LiveMonsters;
            defenderHeroHP = p1HP;
            battleTransform = p2BattleTransform;

            if (deckList.Count > 0)
            {
                topDeckCard = p2Deck[0];
            }
            else
            {
                topDeckCard = null;
            }

        }
        else if (turnState == turn.Player2) //checks if currently Player 2's turn
        {
            turnState = turn.Player1;
            attackTarget = P2Hero;
            deckList = p1Deck;
            handTransform = p1HandTransform;
            handList = p1Hand;
            attackers = p1LiveMonsters;
            defenders = p2LiveMonsters;
            defenderHeroHP = p2HP;
            battleTransform = p1BattleTransform;
            p1HandObject.SetActive(true);
            TurnCountUpdate();

            if (deckList.Count > 0)
            {
                topDeckCard = p1Deck[0];
            }
            else
            {
                topDeckCard = null;
            }
        }

        PhaseUIUpdate();

        if (deckList.Count > 0)
        {
            TurnStart(topDeckCard, deckList, handTransform, handList, attackers, defenders, defenderHeroHP);
        }
        else
        {
            TurnStart(null, deckList, handTransform, handList, attackers, defenders, defenderHeroHP);
        }
    }

    void TurnStart(MonsterCardData topDeckCard, List<MonsterCardData> deckList, Transform handTransform, List<MonsterCardData> handList, List<GameObject> attackers, List<GameObject> defenders, int defenderHeroHP)
    {
        if (deckList.Count > 0) //checks if deck has cards left
        {
            DealCard(topDeckCard, deckList, handTransform, handList);
        }

        if (turnState == turn.Player2 & handList.Count > 0) //checks if P2's turn and has cards in hand
        {
            P2PlayCard(); //AI plays a card
        }

        if (handList.Count == 0) //if player has no cards and hasn't reached max turns, do an attack round
        {
            StartCoroutine(CombatPhase(attackers, defenders, defenderHeroHP)); //calls attack function
        }
    }

    public void DealCard(MonsterCardData topDeckCard, List<MonsterCardData> deckList, Transform handTransform, List<MonsterCardData> handList) //deals card to Player
    {
        MonsterCardData card = Instantiate(topDeckCard); //instantiates an instance of the carddata scriptable object
        MonsterCardUI tempCard = Instantiate(monsterCardTemplate); //instantiates an instance of the card prefab
        tempCard.transform.SetParent(handTransform.transform, false); //moves card to handzone
        tempCard.monsterCardData = card; //sets the cards data to the card dealt
        deckList.Remove(topDeckCard); //removes card from deck list
        handList.Add(card); //adds card to hand list
        DeckUIUpdate();
    }

    public void PlayCard(GameObject playedCard, MonsterCardData monsterCardData , Button button, AudioSource audioSource) //plays a clicked card to the baord
    {
        button.interactable = false; //disables the play card button
        playedCard.transform.SetParent(battleTransform.transform, false); //moves the card to the battlezone
        attackers.Add(playedCard); //adds the card to p1liveMonsters list
        handList.Remove(monsterCardData); //removes card from hand list
        DeckUIUpdate();
        StartCoroutine(CombatPhase(p1LiveMonsters, p2LiveMonsters, p2HP)); //calls AttackRound function and passes it argument parameters
    }

    public void P2PlayCard() //AI plays a random card
    {
        int rng = Random.Range(0, p2Hand.Count); //generates random number
        //MonsterCardUI.PlayClickedCard(monsterCard);
        MonsterCardData card = p2Hand[rng]; //assigns the random number to the card to be played.
        MonsterCardUI playedCard = Instantiate(monsterCardTemplate); //instantiates an instance of the card
        playedCard.monsterCardData = (MonsterCardData)card; //sets the cards data to the card played from hand
        playedCard.button.interactable = false; //disables the play card button
        playedCard.transform.SetParent(battleTransform.transform, false); //moves card to battlezone
        p2LiveMonsters.Add(playedCard.gameObject); //adds the card to p2livemonsters list
        p2Hand.Remove(card); //removes the card from hand list
        DeckUIUpdate();
        StartCoroutine(CombatPhase(p2LiveMonsters, p1LiveMonsters, p1HP)); //calls attack function
    }

    public IEnumerator CombatPhase(List<GameObject> attackers, List<GameObject> defenders, int defenderHeroHP) //function for combat phase
    {
        float combatDelay = 1f;
        p1HandObject.SetActive(false);
        yield return new WaitForSeconds(combatDelay);
        turnPhase = phase.CombatPhase;
        PhaseUIUpdate();
        HeroAttackPhase(attackers, defenders, defenderHeroHP);
    }

    public void HeroAttackPhase(List<GameObject> attackers, List<GameObject> defenders, int defenderHeroHP)
    {
        float endTurnDelay = 1.75f;
        if (attackers.Count > defenders.Count) //checks if attack has more monsters than defender
        {
            for (int i = defenders.Count; i < attackers.Count; i++) //loop repeats for each monster fighting the hero
            {
                MonsterCardUI attacker = attackers[i].GetComponent<MonsterCardUI>();
                animationController.AttackStart(attackers[i], attackTarget);
                defenderHeroHP -= attacker.monsterCardData.attack;
            }
        }
        if (turnState == turn.Player1) //checks if currently Player 1's turn
        {
            p2HP = defenderHeroHP;
        }
        else
        {
            p1HP = defenderHeroHP;
        }

        HeroHPUIUpdate();
        MonsterBattlePhase(attackers, defenders);

        if (defenderHeroHP <= 0) //checks if hero is dead
        {
            print("GameOver by 0 HP");
            GameOver();
            CancelInvoke();
            return;
        }
        if (turnsRemaining == 0 & turnState == turn.Player2)
        {
            print("GameOver by 0 turns remaining");
            GameOver();
            CancelInvoke();
            return;
        }

        print("Turnremaining = " + turnsRemaining);
        Invoke("TurnSwap", endTurnDelay);
    }

    public void MonsterBattlePhase(List<GameObject> attackers, List<GameObject> defenders) //Player 1 monsters attack
    {
        int monsterCombatants;

        if (p1LiveMonsters.Count <= p2LiveMonsters.Count) //compares # of monsters fighting each other. Sets the number of combatants to the length of the shortest list
        {
            monsterCombatants = p1LiveMonsters.Count;
        }
        else
        {
            monsterCombatants = p2LiveMonsters.Count;
        }
        for (int i = 0; i < monsterCombatants; i++) //loop repeats for each monster fighting another monster
        {
            MonsterCardUI monsterAttacker = attackers[i].GetComponent<MonsterCardUI>();
            MonsterCardUI monsterDefender = defenders[i].GetComponent<MonsterCardUI>();
            animationController.AttackStart(attackers[i], defenders[i]);
            monsterDefender.monsterCardData.hp -= monsterAttacker.monsterCardData.attack; //deals damage
            monsterDefender.hpText.text = monsterDefender.monsterCardData.hp.ToString(); //updates UI HP text
            if (monsterDefender.monsterCardData.hp <= 0) //checks if monster is dead
            {
                deadMonsters.Add(monsterDefender.gameObject); //adds to list of dead monsters.
            }
        }
        foreach (GameObject monster in deadMonsters) //destroys dead monsters and removes them from defenders list
        {
            GameObject.Destroy(monster);
            defenders.Remove(monster);
        }
        deadMonsters.Clear();
    }

    public void GameOver() //function for when the game has ended
    {
        if (turnsRemaining == 0) //checks if number of turns has reached max turns
        {
            if (p1HP < p2HP) //checks if p1 has less health
            {
                print("GameOver by 0 turns remaining and P1 lower HP");
                print("turnsremaining " + turnsRemaining);
                print("p1 hp " + p1HP + " p2 hp " + p2HP);
                P2Wins();
            }
            if (p2HP < p1HP) //checks if p2 has less health
            {
                print("GameOver by 0 turns remaining and P2 lower HP");
                print("turnsremaining " + turnsRemaining);
                print("p1 hp " + p1HP + " p2 hp " + p2HP);
                P1Wins();
            }
            if (p1HP == p2HP) //checks if health is tied
            {
                print("GameOver by 0 turns remaining and tied HP");
                print("turnsremaining " + turnsRemaining);
                print("p1 hp " + p1HP + " p2 hp " + p2HP);
                Draw();
            }
            else
            {
                print("GameOver error");
                print("p1 hp " + p1HP + " p2 hp " + p2HP);
            }
        }
        else
        {
            if (p1HP <= 0) //checks if Player1 is dead
            {
                print("GameOver by P1 0 HP");
                print("turnsremaining " + turnsRemaining);
                P2Wins();
            }
            if (p2HP <= 0) //checks if Player2 is dead
            {
                print("GameOver by P2 0 HP");
                print("turnsremaining " + turnsRemaining);
                P1Wins();
            }
        }
    }

    public void P1Wins()
    {
        GameOverObject.SetActive(true);
        gameoverText.text = "Game Over, Player1 Wins";
    }

    public void P2Wins()
    {
        GameOverObject.SetActive(true);
        gameoverText.text = "Game Over, Player2 Wins";
    }

    public void Draw()
    {
        GameOverObject.SetActive(true);
        gameoverText.text = "Game Over, Tied Game";
    }
}
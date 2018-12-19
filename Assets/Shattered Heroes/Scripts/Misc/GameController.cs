using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public enum turn {Player1, Player2, GameOver}; //List of states
    public enum phase {MainPhase, CombatPhase}; //List of states

    turn turnState; //State for current player turn

    public phase turnPhase; //State for current game phase

    int p1HP = 99, p2HP = 99, turnsRemaining = 20, defenderHeroHP, maxTurns;

    public GameObject p1HandObject, GameOverObject;

    GameObject attackTarget;
    
    public Text turnText, p1DeckText, p2DeckText, p1HandText, p2HandText, p1HealthText, p2HealthText, gameoverText; //UI text

    public Transform p1BattleTransform, p2BattleTransform, p1HandTransform, p2HandTransform, handTransform, battleTransform, p1HeroTransform, p2HeroTransform; //Board areas for each players cards

    public List<MonsterCardData> p1Deck, p2Deck, deckList, p1Hand, p2Hand, handList; //lists for the carddata of each decks and hand

    public List<GameObject> p1LiveMonsters, p2LiveMonsters, deadMonsters, attackers, defenders, liveHeroes; //lists for the card prefabs on the board and dead cards

    public List<HeroCardData> heroDeck;

    public AnimationController animationController;

    public MonsterCardUI monsterCardTemplate; //card prefab

    public HeroCardUI heroCardTemplate;

    MonsterCardUI monsterCardUI; //script

    MonsterCardData topDeckCard;
            
    public void Turns() //function for turn states
    {   switch (turnState)
        {
            case turn.Player1:
                break;
            case turn.Player2:
                break;
            case turn.GameOver:
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
        maxTurns = turnsRemaining;
        HeroHPUIUpdate(); //updates UI text
        DealHero(heroDeck[0], p1HeroTransform);
        DealHero(heroDeck[1], p2HeroTransform);
        TurnUpkeep(turn.Player1, liveHeroes[1], p1Deck, p1HandTransform, p1Hand, p1LiveMonsters, p2LiveMonsters, p2HP, p1BattleTransform);
        MainPhase();
        PhaseUIUpdate();
    }

    public void TurnSwap()
    {
        turnPhase = phase.MainPhase;
        topDeckCard = null;

        if (turnState == turn.Player2)
        {
            TurnUpkeep(turn.Player1, liveHeroes[1], p1Deck, p1HandTransform, p1Hand, p1LiveMonsters, p2LiveMonsters, p2HP, p1BattleTransform);
            p1HandObject.SetActive(true);
            TurnCountUpdate();
            MainPhase();
        }
        else
        {
            TurnUpkeep(turn.Player2, liveHeroes[0], p2Deck, p2HandTransform, p2Hand, p2LiveMonsters, p1LiveMonsters, p1HP, p2BattleTransform);
            MainPhase();
        }
        PhaseUIUpdate();
    }

    public void DeckUIUpdate() //updates deck & hand count UI text
    {
        p1DeckText.text = p1Deck.Count.ToString();
        p2DeckText.text = p2Deck.Count.ToString();
        p1HandText.text = p1Hand.Count.ToString();
        p2HandText.text = p2Hand.Count.ToString();
    }

    public void TurnUpkeep(turn currentTurn, GameObject heroObject, List<MonsterCardData> deck, Transform handTrans, List<MonsterCardData> handData, List<GameObject> attackerList, List<GameObject> defenderList, int heroHP, Transform battleTrans)
    {
        turnState = currentTurn;
        attackTarget = heroObject;
        deckList = deck;
        handTransform = handTrans;
        handList = handData;
        attackers = attackerList;
        defenders = defenderList;
        defenderHeroHP = heroHP;
        battleTransform = battleTrans;
        if (deckList.Count > 0)
        {
            topDeckCard = deckList[0];
        }
    }

    void TurnCountUpdate() //increments turn count
    {
        turnsRemaining--;
    }

    void PhaseUIUpdate() //updates current phase UI text
    {
        turnText.text = turnState + " " + turnPhase + ": Turns remaining " + turnsRemaining;
    }

    void HeroHPUIUpdate() //updates Hero HP UI text
    {
        p1HealthText.text = p1HP.ToString();
        p2HealthText.text = p2HP.ToString();
    }

    void MainPhase()
    {
        if (turnsRemaining == maxTurns) //checks if deck has cards left
        {
            DealCard();
            DealCard();
            DealCard();
        }
        else if (deckList.Count > 0) //checks if deck has cards left
        {
            DealCard();
        }

        if (handList.Count == 0) //if player has no cards do an attack round
        {
            StartCoroutine(CombatPhase()); //calls attack function
        }

        if (turnState == turn.Player2 & handList.Count > 0) //checks if P2's turn and has cards in hand
        {
            P2PlayCard(); //AI plays a card
        }
    }

    public void DealHero(HeroCardData hero, Transform heroTransform)
    {
        HeroCardData card = Instantiate(hero);
        HeroCardUI tempCard = Instantiate(heroCardTemplate); //instantiates an instance of the card prefab
        tempCard.transform.SetParent(heroTransform.transform, false); //moves card 
        tempCard.heroCardData = card;
        liveHeroes.Add(tempCard.gameObject);
    }

    public void DealCard() //deals card to Player
    {
        MonsterCardData dealtCard = Instantiate(topDeckCard); //instantiates an instance of the carddata scriptable object
        MonsterCardUI tempCard = Instantiate(monsterCardTemplate); //instantiates an instance of the card prefab
        tempCard.transform.SetParent(handTransform.transform, false); //moves card to handzone
        tempCard.monsterCardData = dealtCard; //sets the cards data to the card dealt
        deckList.Remove(topDeckCard); //removes card from deck list
        handList.Add(dealtCard); //adds card to hand list
        DeckUIUpdate();

        if (deckList.Count > 0)
        {
            topDeckCard = deckList[0];
        }
    }

    public void PlayCard(GameObject playedCard, MonsterCardData monsterCardData , Button button, AudioSource audioSource) //plays a clicked card to the baord
    {
        button.interactable = false; //disables the play card button
        playedCard.transform.SetParent(battleTransform.transform, false); //moves the card to the battlezone
        attackers.Add(playedCard); //adds the card to p1liveMonsters list
        handList.Remove(monsterCardData); //removes card from hand list
        DeckUIUpdate();
        StartCoroutine(CombatPhase()); //calls AttackRound function and passes it argument parameters
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
        attackers.Add(playedCard.gameObject); //adds the card to p2livemonsters list
        handList.Remove(card); //removes the card from hand list
        DeckUIUpdate();
        StartCoroutine(CombatPhase()); //calls attack function
    }

    public IEnumerator CombatPhase() //function for combat phase
    {
        float combatDelay = 1f;
        p1HandObject.SetActive(false);
        yield return new WaitForSeconds(combatDelay);
        turnPhase = phase.CombatPhase;
        PhaseUIUpdate();
        HeroAttackPhase();
    }

    public void HeroAttackPhase()
    {
        float endTurnDelay = 1f;
        if (attackers.Count > defenders.Count) //checks if attack has more monsters than defender
        {
            for (int i = defenders.Count; i < attackers.Count; i++) //loop repeats for each monster fighting the hero
            {
                MonsterCardUI attacker = attackers[i].GetComponent<MonsterCardUI>();
                animationController.AttackStart(attackers[i], attackTarget);
                attacker.PlaySound();
                defenderHeroHP -= attacker.monsterCardData.attack;
                print("defender hp = " + defenderHeroHP);
                print("defender takes " + attacker.monsterCardData.attack + " damage from " + attacker.monsterCardData.cardName);
            }
            if (turnState == turn.Player1) //checks if currently Player 1's turn
            {
                p2HP = defenderHeroHP;
            }
            if (turnState == turn.Player2)
            {
                p1HP = defenderHeroHP;
            }
            HeroHPUIUpdate();
            if (defenderHeroHP <= 0) //checks if hero is dead
            {
                turnState = turn.GameOver;
                GameOver();
                CancelInvoke();
                return;
            }
        }

        if (defenders.Count > 0)
        {
            MonsterBattlePhase(attackers, defenders);
        }

        if (turnsRemaining == 0 & turnState == turn.Player2)
        {
            turnState = turn.GameOver;
            GameOver();
            CancelInvoke();
            return;
        }
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
            animationController.AttackStart(monsterAttacker.gameObject, monsterDefender.gameObject);
            monsterAttacker.PlaySound();
            monsterDefender.monsterCardData.hp -= monsterAttacker.monsterCardData.attack; //deals damage
            monsterDefender.hpText.text = monsterDefender.monsterCardData.hp.ToString(); //updates UI HP text
            print(monsterAttacker.monsterCardData.cardName + " deals " + monsterAttacker.monsterCardData.attack + " damage to " + monsterDefender.monsterCardData.cardName);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public enum turn {Player1, Player2, GameOver}; //List of states
    public enum phase {MainPhase, CombatPhase}; //List of states

    turn turnState; //State for current player turn

    public phase turnPhase; //State for current game phase

    int turnsRemaining = 20, maxTurns;

    public GameObject p1HandObject, GameOverObject, GameOverTextObject;

    GameObject attackTarget, defenderHero;
    
    public Text turnText, p1DeckText, p2DeckText, p1HandText, p2HandText, gameoverText; //UI text

    public Transform p1BattleTransform, p2BattleTransform, p1HandTransform, p2HandTransform, p1HeroTransform, p2HeroTransform; //Board areas for each players cards

    Transform handTransform, battleTransform;

    public List<MonsterCardData> p1Deck, p2Deck, p1Hand, p2Hand; //lists for the carddata of each decks and hand

    List<MonsterCardData> deckList, handList;

    public List<GameObject> p1LiveMonsters, p2LiveMonsters, deadMonsters, attackers, defenders, liveHeroes; //lists for the card prefabs on the board and dead cards

    public List<HeroCardData> heroDeck;

    public AnimationController animationController;

    public MonsterCardPrefab monsterCardTemplate; //card prefab

    public HeroCardPrefab heroCardTemplate;

    float combatDelay = 1f, endTurnDelay = 1f, animationLength = 1f;

    HeroCardPrefab p1Hero, p2Hero;

    MonsterCardPrefab monsterCardPrefab; //script

    MonsterCardData topDeckCard;

    bool isPaused;
            
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

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            PauseMenu();
        }
    }

    public void BeginGame() //function for start of game.
    {
        isPaused = false;
        maxTurns = turnsRemaining;
        Shuffle(p1Deck);
        Shuffle(p2Deck);
        DealHero(heroDeck[0], p1HeroTransform);
        DealHero(heroDeck[1], p2HeroTransform);
        p1Hero = liveHeroes[0].GetComponent<HeroCardPrefab>();
        p2Hero = liveHeroes[1].GetComponent<HeroCardPrefab>();
        P1Turn();
        PhaseUIUpdate();
    }

    void Shuffle(List<MonsterCardData> deck)
    {
        for (int i = 0; i < 500; i++)
        {
            int rng1 = Random.Range(0, deck.Count); //rng1 = 50
            int rng2 = Random.Range(0, deck.Count); //rng2 = 10
            MonsterCardData tempcard = deck[rng1]; //tempcard = card 50
            deck[rng1] = deck[rng2]; //card 50 = card 10
            deck[rng2] = tempcard; //card 10 = card 50
        }
    }

    public void TurnSwap()
    {
        turnPhase = phase.MainPhase;
        topDeckCard = null;

        if (turnState == turn.Player2)
        {
            TurnCountUpdate();
            P1Turn();
        }
        else
        {
            P2Turn();
        }
        PhaseUIUpdate();
    }

    public void P1Turn() //player 1 turn
    {
        p1HandObject.SetActive(true);
        TurnUpkeep(turn.Player1, liveHeroes[1], p1Deck, p1HandTransform, p1Hand, p1LiveMonsters, p2LiveMonsters, liveHeroes[1], p1BattleTransform);
        MainPhase();
    }

    public void P2Turn() //player 2 turn
    {
        TurnUpkeep(turn.Player2, liveHeroes[0], p2Deck, p2HandTransform, p2Hand, p2LiveMonsters, p1LiveMonsters, liveHeroes[0], p2BattleTransform);
        MainPhase();
    }

    public void DeckUIUpdate() //updates deck & hand count UI text
    {
        p1DeckText.text = p1Deck.Count.ToString();
        p2DeckText.text = p2Deck.Count.ToString();
        p1HandText.text = p1Hand.Count.ToString();
        p2HandText.text = p2Hand.Count.ToString();
    }

    public void TurnUpkeep(turn currentTurn, GameObject heroObject, List<MonsterCardData> deck, Transform handTrans, List<MonsterCardData> handData, List<GameObject> attackerList, List<GameObject> defenderList, GameObject defHero, Transform battleTrans)
    { //sets all paramaters for the relevant player at start of turn
        turnState = currentTurn;
        attackTarget = heroObject;
        deckList = deck;
        handTransform = handTrans;
        handList = handData;
        attackers = attackerList;
        defenders = defenderList;
        defenderHero = defHero;
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

    void MainPhase() //Deals cards, plays a card for AI player or waits for human player to play a card, then goes to combat phase
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
            StartCoroutine(CombatPhase());
        }

        if (turnState == turn.Player2 & handList.Count > 0) //checks if its P2's turn and they have cards in hand
        {
            P2PlayCard(); //AI plays a card
        }
    }

    public void DealHero(HeroCardData hero, Transform heroTransform) //Deals hero cards at start of game
    {
        HeroCardData card = Instantiate(hero);
        HeroCardPrefab tempCard = Instantiate(heroCardTemplate); //instantiates an instance of the card prefab
        tempCard.transform.SetParent(heroTransform.transform, false); //moves card 
        tempCard.heroCardData = card;
        liveHeroes.Add(tempCard.gameObject);
    }

    public void DealCard() //deals card to Player
    {
        MonsterCardData dealtCard = Instantiate(topDeckCard); //instantiates an instance of the carddata scriptable object
        MonsterCardPrefab tempCard = Instantiate(monsterCardTemplate); //instantiates an instance of the card prefab
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
        StartCoroutine(CombatPhase()); //calls AttackRound
    }

    public void P2PlayCard() //AI plays a random card
    {
        int rng = Random.Range(0, p2Hand.Count); //generates random number
        MonsterCardData card = p2Hand[rng]; //assigns the random number to the card to be played.
        MonsterCardPrefab playedCard = Instantiate(monsterCardTemplate); //instantiates an instance of the card
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
        p1HandObject.SetActive(false);
        yield return new WaitForSeconds(combatDelay);
        turnPhase = phase.CombatPhase;
        PhaseUIUpdate();
        HeroAttackPhase();
    }

    public void HeroAttackPhase() //creatures with no opposing creature attack the enemy hero
    {
        HeroCardPrefab defHero = defenderHero.GetComponent<HeroCardPrefab>();
        if (attackers.Count > defenders.Count) //checks if attack has more monsters than defender
        {
            for (int i = defenders.Count; i < attackers.Count; i++) //loop repeats for each monster fighting the hero
            {
                MonsterCardPrefab attacker = attackers[i].GetComponent<MonsterCardPrefab>();
                StartCoroutine(animationController.AttackMove(attackers[i], attackTarget, animationLength));

                attacker.PlaySound(); //plays sound
                defHero.heroCardData.hp -= attacker.monsterCardData.attack; //deals damage
                defHero.hpText.text = defHero.heroCardData.hp.ToString(); //updates UI HP text
                print(attacker.monsterCardData.cardName + " deals " + attacker.monsterCardData.attack + " to defending hero");

                if (defHero.heroCardData.hp <= 66)
                {
                    defHero.heroCardData.HeroArtUpdate(defHero, defHero.heroCardData.artSprite2);
                }
                if (defHero.heroCardData.hp <= 33)
                {
                    defHero.heroCardData.HeroArtUpdate(defHero, defHero.heroCardData.artSprite3);
                }
                if (defHero.heroCardData.hp <= 0)
                {
                    defHero.heroCardData.HeroArtUpdate(defHero, defHero.heroCardData.artSprite4);
                }
            }
            if (turnState == turn.Player1) //checks if currently Player 1's turn
            {
                p1Hero.heroCardData.hp = defHero.heroCardData.hp;
            }
            if (turnState == turn.Player2)
            {
                p2Hero.heroCardData.hp = defHero.heroCardData.hp;
            }
            if (defHero.heroCardData.hp <= 0) //checks if hero is dead
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

    public void MonsterBattlePhase(List<GameObject> attackers, List<GameObject> defenders) //creatures with an opposing monster fight each other
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
            MonsterCardPrefab monsterAttacker = attackers[i].GetComponent<MonsterCardPrefab>();
            MonsterCardPrefab monsterDefender = defenders[i].GetComponent<MonsterCardPrefab>();
            StartCoroutine(animationController.AttackMove(monsterAttacker.gameObject, monsterDefender.gameObject, animationLength));
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

    void PauseMenu() //opens a menu to quit or restart
    {
        if(isPaused == false)
        {
            isPaused = true;
            GameOverObject.SetActive(true);
        }
        else
        {
            isPaused = false;
            GameOverObject.SetActive(false);
        }
    }

    public void GameOver() //function for when the game has ended
    {
        GameOverObject.SetActive(true); //enables gameover menu
        GameOverTextObject.SetActive(true); //enables gameover text
        if (turnsRemaining == 0) //checks if number of turns has reached max turns
        {
            if (p1Hero.heroCardData.hp < p2Hero.heroCardData.hp) //checks if p1 has less health
            {
                GameOverMessage("Game Over, Player2 Wins");
            }
            if (p2Hero.heroCardData.hp < p1Hero.heroCardData.hp) //checks if p2 has less health
            {
                GameOverMessage("Game Over, Player1 Wins");
            }
            if (p1Hero.heroCardData.hp == p2Hero.heroCardData.hp) //checks if health is tied
            {
                GameOverMessage("Game Over, Tied Game");
            }
            else
            {
                print("GameOver Error. P1 HP " + p1Hero.heroCardData.hp + " P2 HP " + p2Hero.heroCardData.hp);
            }
        }
        else
        {
            if (p1Hero.heroCardData.hp <= 0) //checks if Player1 is dead
            {
                GameOverMessage("Game Over, Player2 Wins");
            }
            if (p2Hero.heroCardData.hp <= 0) //checks if Player2 is dead
            {
                GameOverMessage("Game Over, Player1 Wins");
            }
        }
    }

    public void GameOverMessage(string winText) //updates game over UI text
    {
        gameoverText.text = winText;
    }
}
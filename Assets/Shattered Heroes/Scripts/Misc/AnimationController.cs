using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    float animationLength = 1f;

    public GameController gameController;

    GameObject attacker, defender;
    
    public void AttackStart(GameObject attacker, GameObject defender)
    {
        StartCoroutine(AttackMove(attacker, defender, animationLength));
    }

    public IEnumerator AttackMove(GameObject attacker, GameObject defender, float duration) //function for animating movement of attacking card
    {
        Vector3 startPos, targetPos;
        startPos = attacker.transform.position; //stores vector of this cards starting position 
        targetPos = defender.transform.position; //stores vector of this cards target position
        float moveElapsed = 0f; //amount of time passed
        float returnElapsed = 0f; //amount of time passed

        if (gameController.turnPhase == GameController.phase.CombatPhase & attacker != null & defender != null) //checks it is combat phase and card hasn't attacked already
        {
            while (moveElapsed < animationLength) //checks if time passed is less than animation length
            {
                moveElapsed = Mathf.Min(moveElapsed + (Time.deltaTime / duration), animationLength);
                attacker.transform.position = Vector3.Lerp(startPos, targetPos, moveElapsed);
                yield return null;
            }

            while (returnElapsed < animationLength & moveElapsed >= animationLength)
            {
                returnElapsed = Mathf.Min(returnElapsed + (Time.deltaTime / duration), animationLength);
                attacker.transform.position = Vector3.Lerp(targetPos, startPos, returnElapsed);
                yield return null;
            }
        }
    }
}
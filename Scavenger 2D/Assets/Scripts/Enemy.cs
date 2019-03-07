using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start ()
    {
        GameManager.instance.AddEmenyToList(this);                          //adds the enemy script to the GM list, for enemy movement control by GM 
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;              //find the players position
        base.Start();
	}
	
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)                       //only use attemptmove every other frame: if skipMove is true
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)        //if x position of player and enemy are nearly (epsilon) the same
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;                   //and y player pos is greater than y enemy pos move 1 (up) otherwise move -1 (down)
        }
        else
        {
            xDir = target.position.y > transform.position.y ? 1 : -1;                   //same for x movement
        }

        AttemptMove<Player>(xDir, yDir);

    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyHit");

        hitPlayer.LoseFood(playerDamage);

        SoundManager.instance.RandomizeSFX(enemyAttack1, enemyAttack2);
    }
}

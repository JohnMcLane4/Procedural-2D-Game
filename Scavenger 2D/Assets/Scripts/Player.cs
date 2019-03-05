using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;

	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;       //get the count of food froim gamemanager

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;       //store the change in food in the gamemanager, to keep the int between level changes   
    }

    // Update is called once per frame
    void Update ()
    {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;                                             //store movement as 1 or -1 
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)                                            //only vertical or horizontal movement, never both at the same time, no diagonal movement
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)                   //attempt to move (non zero in vert or horiz)
        {
            AttemptMove<Wall>(horizontal, vertical);            //call attemptmove <maybe interact with> (movement direction)
        }

    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;                                                             //loose one unit food per step

        base.AttemptMove<T>(xDir, yDir);                                    // call attemptmove

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;                           //players turn has ended
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;       //declare hitwall as component of wall
        hitWall.DamageWall(wallDamage);         //hitwall calls damagewall from wall component with var walldamage
        animator.SetTrigger("playerChop");      
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()                      //check if gameover, if food is 0 or less
    {
        if(food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    private Vector2 touchOrigin = -Vector2.one; //record where the finger starts to touch the screen, initialised to offscreen position

	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;       //get the count of food from gamemanager

        foodText.text = "Food: " + food;

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

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)                                            //only vertical or horizontal movement, never both at the same time, no diagonal movement
        {
            vertical = 0;
        }
#else
        if(Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];   //save first touch in mytouch / support fo only one finger

            if(myTouch.phase == TouchPhase.Began)   //save the touch phase begin to touchorigin
            {
                touchOrigin = myTouch.position;
            }      
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)        
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;           //calculate touch direction from end and begin of touch
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))            //swipe more horizontal than vertical...
                {
                    horizontal = x > 0 ? 1 : -1;            //set horizontal to one if > 0 otherwise to -1
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }

#endif

        if (horizontal != 0 || vertical != 0)                   //attempt to move (non zero in vert or horiz)
        {
            AttemptMove<Wall>(horizontal, vertical);            //call attemptmove <maybe interact with> (movement direction)
        }

    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;                                                             //loose one unit food per step
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);                                    // call attemptmove

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSFX(moveSound1, moveSound2);
        }

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
            foodText.text = "+" + pointsPerFood + " Food " + food;
            SoundManager.instance.RandomizeSFX(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food " + food;
            SoundManager.instance.RandomizeSFX(drinkSound1, drinkSound2);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()                      //check if gameover, if food is 0 or less
    {
        if(food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}

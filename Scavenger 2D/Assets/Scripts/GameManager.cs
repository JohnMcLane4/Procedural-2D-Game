using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;  //from everwhere accessable singleton instance
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private BoardManager boardScript;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup = true;

    void Awake()
    {
        if (instance == null)       //making sure we only have one gamemanager
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);     //keep gamemanager from getting destroyed on sceneload to keep up with score e.g.
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {        
        SceneManager.sceneLoaded += OnSceneLoaded;  //register a callback everytime a scene is loaded(not at the first time)
    }
    
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)        //start coroutine if its not playersturn or enemiesmoving is true
            return;

        StartCoroutine(MoveEnemies());

    }

    public void AddEmenyToList(Enemy script)
    {
        enemies.Add(script);                    //enemies register to gamemanager in that list, so that GM can give movement commands
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);         //wait
        if (enemies.Count == 0)                              //no enemies spawned 
        {
            yield return new WaitForSeconds(turnDelay);     //wait when no enemies spawned
        }

        for (int i = 0; i < enemies.Count; i++)             //loop through enemies list 
        {
            enemies[i].MoveEnemy();                                 // call moveEnemy for all enemies...
            yield return new WaitForSeconds(enemies[i].moveTime);   //...but with a delay
        }

        playersTurn = true;
        enemiesMoving = false;                                      //end of coroutine, next frame we call it again
    }
}
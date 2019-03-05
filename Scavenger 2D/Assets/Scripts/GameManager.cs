using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;  //from everwhere accessable singleton instance
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private int level = 3;

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

        boardScript = GetComponent<BoardManager>();     
        InitGame(); 
    }

    void InitGame()
    {
        boardScript.SetupScene(level);
    }

    public void GameOver()
    {
        enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

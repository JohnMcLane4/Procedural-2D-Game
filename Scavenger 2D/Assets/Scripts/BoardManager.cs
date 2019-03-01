using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8; //Size of gameboard: 8x8
    public int rows = 8;
    public Count wallCount = new Count(5, 9);   // number of walls in level (not outside walls included)
    public Count foodCount = new Count(1, 5);   // number of food items
    public GameObject exit;
    public GameObject[] floorTiles; //Arrays for all tiles
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;  //to keep hirachy clean, parent of all tiles
    private List<Vector3> gridPositions = new List<Vector3>();  // list of gridpositions

    void InitialiseList()
    {
        gridPositions.Clear();  //clear the gridpositions list

        for (int x = 1; x < columns - 1; x++)       //loop through x columns and y rows, -1 to leave the tiles next to the outer walls empty. otherwise impassible levels are possible
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));       //tiles for enemies, food, water
            }
        }
    }

    void BoardSetup()   //outer walls and floor tiles
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)      //+1 outer walls should be created outside of the 8x8 gameboard 
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];  //instantiate floortiles from floortiles array
                if (x == -1 || x == columns || y == -1 || y == rows)     //check if position is (...)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];     //instantiate outerwalltiles
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;     //tiles the code chooses(outerwall,floortiles) getting instantiated at position (x,y) from for loop and without rotation(quaternion)

                instance.transform.SetParent(boardHolder);  //setparent to boardholder for cleaner hirachy
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);     //randonnummer between 0 and gridposition
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);                        //to not spawn on the same position, remove random position from gridpos from list
        return randomPosition;
    }

    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);       //how many objects we spawn in level

        for (int i = 0; i < objectCount; i++)                       //looping till objectcount -1 
        {
            Vector3 randomPosition = RandomPosition();                              //and calling a random position through randomPosition()
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];   //select random tile 
            Instantiate(tileChoice, randomPosition, Quaternion.identity);           //instantiate tile at choosen random pos
        }
    }   

    public void SetupScene (int level)      //public method to start all other methods
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}

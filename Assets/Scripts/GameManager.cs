using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIControls uiCntr;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject mazeRoot;
    public bool inHole = false;
    private List<GameObject> mazesList = new List<GameObject>();
    private List<GameObject> triggersList = new List<GameObject>();
    private List<GameObject> spawnPositions = new List<GameObject>();
    private int currentMazeIndex = 0;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Rigidbody ballRb;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();

        // Get all maze children
        foreach (Transform child in mazeRoot.transform)
            mazesList.Add(child.gameObject);
        
        // Find transitionTrigger and spawnBallPosition in each maze
        foreach (GameObject maze in mazesList)
        {
            Transform trigger = maze.transform.Find("transitionTrigger");
            if (trigger != null) triggersList.Add(trigger.gameObject);
            
            Transform spawn = maze.transform.Find("spawnBallPosition");
            if (spawn != null) spawnPositions.Add(spawn.gameObject);
        }
        
        // Disable all mazes first
        foreach (GameObject maze in mazesList)
            maze.SetActive(false);
        
        // Enable only the first maze
        mazesList[currentMazeIndex].SetActive(true);
        
        // Enable and Position the ball at spawn point
        ball.SetActive(true);
        ball.transform.position = spawnPositions[currentMazeIndex].transform.position;
        ball.transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (inHole) TransitionToNextMaze();
    }

    public void TransitionToNextMaze()
    {
        inHole = false;

        // switching to next maze
        mazesList[currentMazeIndex].SetActive(false);
        currentMazeIndex++;
        if (currentMazeIndex == mazesList.Count) currentMazeIndex = 0;
            mazesList[currentMazeIndex].SetActive(true);

        // reposition the ball
        ball.transform.position = spawnPositions[currentMazeIndex].transform.position;
        ball.transform.rotation = Quaternion.identity;

        // resetting velocity
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }
    public void RespawnBallOutOfMaze()
    {
        // reposition the ball
        ball.transform.position = spawnPositions[currentMazeIndex].transform.position;
        ball.transform.rotation = Quaternion.identity;

        // resetting velocity
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }
}

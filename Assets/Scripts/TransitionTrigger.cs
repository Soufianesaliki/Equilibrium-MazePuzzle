using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    [SerializeField] private GameManager manager;

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.CompareTag("Hole"))
            manager.TransitionToNextMaze();
    }
    void OnTriggerExit(Collider trigger)
    {
        if (trigger.CompareTag("Boundary"))
            manager.RespawnBallOutOfMaze();
    }
}

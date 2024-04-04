using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovementSequencer : MonoBehaviour
{
    //list of the objects as the wayspoints
    public List<GameObject> gameobjectWayPoints = new List<GameObject>();

    //the object we want to move along this dumb line
    public GameObject chosenObjectToMove;

    //the index of the next wayppoint
    public int indexOfNextWaypoint;


    //status of movement fields
    public int lastCompletedWaypoint
    {
        get; private set;
    }
    public int percentCompleted //total percent of completion from 1-100
    { 
        get; private set;
    }

    //unity event for messaging current sequence of events
    //the int number is the current waypoint that has just been reached
    public UnityEvent<int> completedWaypoint = new();

    //control fields for the speed, direction (forward or backweard) and use an animation curve to modify speed for specfic areas of the travel :)
    [SerializeField] private AnimationCurve speedCurveAlongTravel;
    public bool goingForward = true;
    public float speedOfTravel;


    private void Start()
    {
        lastCompletedWaypoint = 0;
        indexOfNextWaypoint = 0;
        StartMovementToNextWaypoint();
    }

    //movement of object region
    void StartMovementToNextWaypoint()
    {
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 1f)
        {
            float speed = speedOfTravel * speedCurveAlongTravel.Evaluate(timeElapsed);
            // Calculate the lerp value based on time and speed
            float lerpValue = Mathf.Lerp(0f, 1f, timeElapsed * speed);

            // Use lerp to interpolate between start and end positions
            chosenObjectToMove.transform.position = Vector3.Lerp(gameobjectWayPoints[lastCompletedWaypoint].transform.position, 
                                                                 gameobjectWayPoints[indexOfNextWaypoint].transform.position, 
                                                                 lerpValue);

            // Update timeElapsed based on deltaTime
            timeElapsed += Time.deltaTime;

            // Wait for next frame before continuing
            yield return null;
        }

        // Ensure final position is reached
        chosenObjectToMove.transform.position = gameobjectWayPoints[indexOfNextWaypoint].transform.position;
        completedWaypoint.Invoke(indexOfNextWaypoint);
        UpdateCompletionWaypoints();
    }


    //sequence selection
    void UpdateCompletionWaypoints()
    {
        lastCompletedWaypoint = indexOfNextWaypoint;

        if (indexOfNextWaypoint == gameobjectWayPoints.Count - 1)
        {
            // Reverse direction when reaching the end
            indexOfNextWaypoint = lastCompletedWaypoint - 2; // Adjust for last completed waypoint
            goingForward = !goingForward; // Toggle direction flag
        }
        if (goingForward)
        {
            indexOfNextWaypoint++;
        }
        StartMovementToNextWaypoint();
    }
}

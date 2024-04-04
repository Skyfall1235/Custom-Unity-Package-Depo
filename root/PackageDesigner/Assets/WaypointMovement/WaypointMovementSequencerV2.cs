using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovementSequencerV2 : MonoBehaviour
{
    public List<Transform> transformsToLoadToStack = new List<Transform>();
    public Stack<Transform> initStack = new Stack<Transform>();
    public Transform currentTransform
    {
        get 
        { 
            if(goingForward)
            { 
                return initStack.Peek();
            }
            else
            { 
                return invertedStack.Peek();
            }
        }
    }
    public Stack<Transform> StackToPullFrom
    {
        get
        {
            if (goingForward)
            {
                return initStack;
            }
            else 
            { 
                return invertedStack;
            }
        }
    }

    public Stack<Transform> invertedStack = new Stack<Transform>();

    //the object we want to move along this dumb line
    public GameObject chosenObjectToMove;

    //this will be modified to show the location oft he transform out of al lthe items in both stacks
    public int lastCompletedWaypoint
    {
        get; private set;
    }

    public bool goingForward = true;
    public float speedOfTravel;

    //the int number is the current waypoint that has just been reached
    public UnityEvent<int> completedWaypoint = new();

    private void Start()
    {
        LoadListToStack();
    }

    private void LoadListToStack()
    {
        for (int i = 0; i < transformsToLoadToStack.Count; i++)
        {
            initStack.Push(transformsToLoadToStack[i]);
        }
    }


    private IEnumerator MoveObject()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 1f)
        {
            float speed = speedOfTravel;
            // Calculate the lerp value based on time and speed
            float lerpValue = Mathf.Lerp(0f, 1f, timeElapsed * speed);

            // Use lerp to interpolate between start and end positions
            /*chosenObjectToMove.transform.position = Vector3.Lerp(gameobjectWayPoints[lastCompletedWaypoint].transform.position,
                                                                 gameobjectWayPoints[indexOfNextWaypoint].transform.position,
                                                                 lerpValue);*/

            // Update timeElapsed based on deltaTime
            timeElapsed += Time.deltaTime;

            // Wait for next frame before continuing
            yield return null;
        }

        // Ensure final position is reached
        //chosenObjectToMove.transform.position = gameobjectWayPoints[indexOfNextWaypoint].transform.position;
        //completedWaypoint.Invoke(indexOfNextWaypoint);

        ShiftStackToNextItem();
        StartMovementToNextWaypoint();
    }

    private void ShiftStackToNextItem()
    {
        //determine if current direction of movement is posible, and react accordingly
        if(StackToPullFrom.Count == 0)
        {
            goingForward = !goingForward;
        }

        //shift the stacks as needed and reassign the current transform to the top of the stack we are currently pulling from

        //assign going forward bool as needed
    }

    void StartMovementToNextWaypoint()
    {
        StartCoroutine(MoveObject());
    }
}

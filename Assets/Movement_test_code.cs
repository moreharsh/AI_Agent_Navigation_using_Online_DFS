using System.Collections.Generic;
using UnityEngine;

public class Movement_test_code : MonoBehaviour
{
    public float movementSpeed = 5f;

    private bool left_open = false;
    private bool right_open = false;
    private const float forwardRayDistance = 1.5f;
    private const float sideRayDistance = 3f;
    private const float centerRayDistance = 5f;

    private enum State { Exploring, Backtracking }
    private State currentState = State.Exploring;

    private Stack<Vector3> pathStack = new Stack<Vector3>();


    void Start() { }

    void Update()
    {
        CenterAgentBetweenWalls();
        switch (currentState)
        {
            case State.Exploring:
                NavigateMaze();
                break;
            case State.Backtracking:
                while (pathStack.Count > 0)
                {
                    Vector3 targetPosition = pathStack.Pop();
                    Debug.Log("Backtrack to: " + targetPosition);
                    if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                    }
                    // process value
                }
                break;
        }
    }

    void CenterAgentBetweenWalls()
    {
        Vector3 position = transform.position;
        Ray rayLeft = new Ray(position, -transform.right);
        Ray rayRight = new Ray(position, transform.right);

        if (Physics.Raycast(rayLeft, out RaycastHit hitLeft, centerRayDistance) &&
            Physics.Raycast(rayRight, out RaycastHit hitRight, centerRayDistance) &&
            hitLeft.transform.CompareTag("Wall") && hitRight.transform.CompareTag("Wall"))
        {
            Vector3 midPoint = (hitLeft.point + hitRight.point) / 2f;
            Vector3 targetPosition = new Vector3(midPoint.x, position.y, midPoint.z);
            transform.position = Vector3.Lerp(position, targetPosition, Time.deltaTime * 5f);
        }
    }

    void NavigateMaze()
    {
        Vector3 position = transform.position;
        Ray rayForward = new Ray(position, transform.forward);
        Ray rayLeft = new Ray(position, -transform.right);
        Ray rayRight = new Ray(position, transform.right);

        bool isForwardBlocked = Physics.Raycast(rayForward, out RaycastHit hitForward, forwardRayDistance) &&
                                hitForward.transform.CompareTag("Wall");
        bool isLeftBlocked = Physics.Raycast(rayLeft, out RaycastHit hitLeft, sideRayDistance) &&
                             hitLeft.transform.CompareTag("Wall");
        bool isRightBlocked = Physics.Raycast(rayRight, out RaycastHit hitRight, sideRayDistance) &&
                              hitRight.transform.CompareTag("Wall");

        if (isForwardBlocked)
        {
            if (isLeftBlocked)
            {
                if (isRightBlocked)
                {
                    RotateAgent(180f);
                    currentState = State.Backtracking;
                }
                else
                {
                    RotateAgent(90f); // Right
                }
            }
            else
            {
                RotateAgent(-90f); // Left
            }
        }
        else
        {
            pathStack.Push(transform.position);
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

            if (Physics.Raycast(rayLeft, sideRayDistance))
            {
                if(left_open)
                {
                    Debug.Log("Left closed: " + position);
                    left_open = false;
                }
            }
            else
            {
                if(!left_open)
                {
                    Debug.Log("Left open: " + position);
                    left_open = true;
                }
            }
            if (Physics.Raycast(rayRight, sideRayDistance))
            {
                if(right_open)
                {
                    Debug.Log("Right closed: " + position);
                    right_open = false;
                }
            }
            else
            {
                if(!right_open)
                {
                    Debug.Log("Right open: " + position);
                    right_open = true;
                }
            }
        }
    }

    void RotateAgent(float angle)
    {
        transform.Rotate(Vector3.up * angle);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
        Gizmos.DrawRay(transform.position, -transform.right * 2f);
        Gizmos.DrawRay(transform.position, transform.right * 2f);
    }
}

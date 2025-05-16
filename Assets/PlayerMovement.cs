using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 10f;
    public NavMeshAgent agent;

    private const float centerRayDistance = 5f;
    private const float forwardRayDistance = 1.5f;
    private const float sideRayDistance = 5f;

    private enum State { Exploring, Backtracking, Ideal }
    private State currentState = State.Exploring;

    private Stack<(Vector3 position, int angle)> pathStack = new Stack<(Vector3, int)>();

    private (Vector3 position, int angle) backtrackto;

    
    private bool left_open = false;
    private bool right_open = false;
    private bool is_backtracking = false;
    private int left_open_counter = 0;
    private int right_open_counter = 0;
    private Vector3 open_left_position, open_right_position;

    void Start() 
    { 
        pathStack.Push((transform.position, 0));
    }

    void Update()
    {
        CenterAgentBetweenWalls();
        switch (currentState)
        {
            case State.Exploring:
                NavigateMaze();
                break;
            case State.Backtracking:
                if(pathStack.Count > 0 && !is_backtracking)
                {
                    backtrackto = pathStack.Pop(); 
                    agent.updateRotation=false;
                    agent.speed = 10f;
                    is_backtracking = true;
                    agent.SetDestination(backtrackto.position);
                }

                if(is_backtracking && Vector3.Distance(transform.position, backtrackto.position) < 0.5f)
                {
                    currentState = State.Exploring;
                    RotateAgent(backtrackto.angle);
                    agent.speed = 0f;
                    is_backtracking = false;
                }
                break;
            case State.Ideal:
                break;
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

        if(isForwardBlocked)
        {
            if(isRightBlocked)
            {
                if(isLeftBlocked)
                {
                    currentState = State.Backtracking;
                }
                else
                {
                    agent.updateRotation=true;
                    RotateAgent(-90);
                    left_open_counter += 1;
                    left_open = false;
                    right_open = false;
                    agent.updateRotation=false;
                }
            }
            else
            {
                agent.updateRotation=true;
                RotateAgent(90);
                right_open_counter += 1;
                left_open = false;
                right_open = false;
                agent.updateRotation=false;
            }
            if(!isRightBlocked && !isLeftBlocked)
            {
                pathStack.Push((transform.position, -90));
            }
        }
        else
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            
            if (Physics.Raycast(rayLeft, sideRayDistance))
            {
                if(left_open && (currentState == State.Exploring) && left_open_counter == 1)
                {
                    Vector3 mid_point = (open_left_position + position)/2;
                    pathStack.Push((mid_point, -90));
                    left_open = false;
                    left_open_counter = 0;
                }
            }
            else
            {
                if(!left_open && (currentState == State.Exploring))
                {
                    open_left_position = position;
                    left_open = true;
                    left_open_counter += 1;
                }
            }
            
            if (Physics.Raycast(rayRight, sideRayDistance))
            {
                if(right_open && (currentState == State.Exploring) && right_open_counter == 1)
                {
                    Vector3 mid_point = (open_right_position + position)/2;
                    pathStack.Push((mid_point, 90));                    
                    right_open = false;
                    right_open_counter = 0; 
                }
            }
            else
            {
                if(!right_open && (currentState == State.Exploring))
                {
                    open_right_position = position;
                    right_open = true;
                    right_open_counter += 1;
                }
            }
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
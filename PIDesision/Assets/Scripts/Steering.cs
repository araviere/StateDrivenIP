using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    enum State
    {
        attack,
        roam,
        sleep,
        eat,
        work,
    }
    State pIState;
    public string currentState;

    float maxSleep = 120;
    public float sleep = 120;
    public int gold = 0;

    float maxHunger = 100;
    public float hunger = 100;
    int maxRoamingAmount = 5;
    public int roamingAmound = 0;


    public Vector3 velocity;
    private Vector3 targetVelocity;
    private Quaternion rotateDirection;
    private Vector3 direction;

    public List<Node> mapLocations;
    public MonoBehaviour target;
    public MonoBehaviour detectedEnemy;
    public Node sleepingBedSpot;
    public Node eathingSpot;
    public Node workingSpot;
    public Node destinationNode;
    public List<Node> path;

    public Navigator navigator;
    System.Random random = new System.Random();

    public Node currentNode;

    public float maxForwardSpeed;
    public float maxSideWaysSpeed;
    public float mass;
    public float rotationSpeed = 3.0f;

    public bool isFleeing = false;

    void SetState(State newState) { pIState = newState; }
    void Start()
    {
        DecideCurrentState();
    }
    void FixedUpdate()
    {
        switch (pIState)
        {
            case State.attack: Attack(); break;
            case State.roam: Roam(); break;
            case State.sleep: Sleep(); break;
            case State.eat: Eat(); break;
            case State.work: Work(); break;
        }
    }

    void DecideCurrentState()
    {

        switch (1)
        {
            case int n when (detectedEnemy != null && pIState != State.roam): SetState(State.attack); break;
            case int n when (sleep <= maxSleep / 5): SetState(State.sleep); MakePath(sleepingBedSpot); break;
            case int n when (hunger <= maxHunger / 3): SetState(State.eat); MakePath(eathingSpot); break;
            default:
                SetState(State.work);
                MakePath(workingSpot); break;
        }
        currentState = pIState.ToString();
        Debug.Log(pIState);
    }

    void NextNode()
    {
        /*try
        {
            currentNode = (Node)target;
        }
        catch { }*/
        if (path.Count > 0)
        {
            target = (MonoBehaviour)path[path.Count - 1];
            path.RemoveAt(this.path.Count - 1);
        }
    }

    void MakePath(Node endNode)
    {
        path.Clear();
        destinationNode = endNode;
        Debug.Log(currentNode);
        path = navigator.CalculatePath(currentNode, endNode);
        path.Add(currentNode);
        NextNode();
    }
    void Roam()
    {
        if (path.Count == 0)
        {
            if (roamingAmound >= maxRoamingAmount) { detectedEnemy = null; roamingAmound = 0; DecideCurrentState(); }
            else
            {
                MakePath(mapLocations[random.Next(0, mapLocations.Count - 1)]);
                roamingAmound++;
            }
        }
        Movement();
    }
    void Sleep()
    {
        if (sleep >= maxSleep / 2 && hunger <= maxHunger / 6)
        {
            DecideCurrentState();
        }
        if (sleep >= maxSleep) { sleep = maxSleep; DecideCurrentState(); }
        else if (currentNode == sleepingBedSpot)
        {
            sleep += (5 * Time.deltaTime);
        }
        Movement();
    }
    void Eat()
    {
        if (sleep <= maxSleep / 8 && hunger >= maxHunger / 2)
        {
            if(currentNode == eathingSpot)
            {
                gold -= 200;
            }
            DecideCurrentState();
        }
        if (hunger >= maxHunger)
        {
            hunger = maxHunger;
            gold -= 200;
            DecideCurrentState();
        }
        else if (currentNode == eathingSpot)
        {
            hunger += (20 * Time.deltaTime);
        }

        Movement();
    }
    void Work()
    {
        if (sleep <= maxSleep / 5 || hunger <= maxHunger / 3)
        {
            DecideCurrentState();
        }

        else if (currentNode == workingSpot)
        {
            gold++;
            sleep = Math.Max(0, sleep - 2 * Time.deltaTime);
            hunger = Math.Max(0, hunger - 1 * Time.deltaTime);
        }
        Movement();
    }

    void Attack()
    {
        if (target == null || Vector3.Distance(transform.position, target.transform.position) > 50f)
        {
            SetState(State.roam);
            currentState = pIState.ToString();
            Debug.Log(pIState);
        }
        else if (Vector3.Distance(transform.position, target.transform.position) < 50f) { target = detectedEnemy; Movement(); }            
    }

    void Movement()
    {
        if (path.Count == 0 && (currentNode != destinationNode && target != destinationNode) && pIState != State.roam)
        {
            DecideCurrentState();
        }
        hunger = Math.Max(0, hunger - 1 * Time.deltaTime);
        sleep = Math.Max(0, sleep - 1 * Time.deltaTime);
        LookAt();
        targetVelocity = Vector3.Normalize(target.transform.position - this.transform.position) * 0.25f;

        velocity.z = 5f;

        if (isFleeing == false && Vector3.Distance(transform.position, target.transform.position) > 1.5f || isFleeing == true && Vector3.Distance(transform.position, target.transform.position) < 50)
        {
            transform.Translate(velocity * Time.deltaTime);
        }
        else { NextNode(); }

    }

    void LookAt()
    {
        direction = (target.transform.position - this.transform.position).normalized;
        rotateDirection = Quaternion.LookRotation(isFleeing ? -direction : direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateDirection, Time.deltaTime * rotationSpeed);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Robot
{
    public bool programInProgress = false;
    public bool needToUpdateLog = false;
    public bool programPaused;

    public RobotProgram program;
    public ProgramType programType;
    public Graph graph;

    public Transform robotGameobject;
    public robotControl robotScript;

    public CameraMovementManager cameraManager;
    public AudioManager audioManager;

    public node currentNode;
    public node targetNode;

    public float waitTime;
    float currentTime;

    public node startNode;

    

    public List<string> log;

    public Robot(ProgramType _programType)
    {
        programType = _programType;

        
    }

    public void startProgram()
    {
        programInProgress = true;
        waitTime = 2f;
        currentTime = waitTime;
        foreach (node Node in graph.nodes)
        {
            Node.script.resetColour();
        }
        foreach (vertex Vertex in graph.vertices)
        {
            Vertex.script.resetColour();
        }

        cameraManager = GameObject.FindGameObjectWithTag("CameraMovementManager").GetComponent<CameraMovementManager>();
        
        switch (programType)
        {
            case ProgramType.prims:
                program = new primsAlgorithm();
                break;
            case ProgramType.kruskals:
                program = new kruskalsAlgorithm();
                break;
        }

        log.Clear();

        startNode.script.resetColour();

        program.stage = 0;
       

    }

    public void endProgram()
    {
        programInProgress = false;
        programPaused = false;
    }
    public void pauseProgram()
    {
        if (programInProgress)
        {
            audioManager.Play("PauseProgram");
            programInProgress = false;
            programPaused = true;
            log.Add("PROGRAM PAUSED");
            needToUpdateLog = true;
        }

    }

    public void unPauseProgram()
    {
        if (programPaused)
        {
            programInProgress = true;
            programPaused = false;
            log.Add("PROGRAM UNPAUSED");
            needToUpdateLog = true;
        }
    }

    public void carryOutProgram()
    {
        if (currentTime >= waitTime && programInProgress)
        {
            programInProgress = program.process(this);
            audioManager.Play("NextInstruction");
            needToUpdateLog = true;
            currentTime = 0;
        }
        else { currentTime += Time.deltaTime; }

    }

    public void setNodePos()
    {
        robotGameobject.position = currentNode.position;
    }



}

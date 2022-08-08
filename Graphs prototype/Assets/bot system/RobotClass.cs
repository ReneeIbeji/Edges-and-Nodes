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

    RobotProgram program;
    public ProgramType programType;
    public Graph graph;

    public Transform robotGameobject;
    public robotControl robotScript;

    public CameraMovementManager cameraManager;

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
        }

        log.Clear();

        startNode.script.resetColour();

        program.stage = 0;
       

    }
    public void pauseProgram()
    {
        if (programInProgress)
        {
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

[System.Serializable]
public class RobotProgram
{

    public int stage;
    public node startNode;
    public RobotProgram()
    {
        stage = 0;
    }

    public virtual bool process(Robot robot)
    {
        return true;
    }
}

public enum ProgramType
{
    prims,
}

public class primsAlgorithm : RobotProgram
{
    List<vertex> edgesList = new List<vertex>();
    List<vertex> finalEdges = new List<vertex>();

    List<node> nodesCovered = new List<node>();

    vertex smallestEdge;
    public override bool process(Robot robot)
    {

        switch (stage)
        {
            case 0:
                if (robot.startNode != null)
                {
                    robot.currentNode = robot.startNode;
                }
                else
                {
                    robot.currentNode = robot.graph.nodes.ToArray()[0];
                }
                robot.robotGameobject.position = robot.currentNode.position;
                edgesList = new List<vertex>();
                finalEdges = new List<vertex>();
                stage++;

                nodesCovered.Add(robot.currentNode);
                robot.currentNode.script.hightlightPrimary();
                robot.log.Add("starting at node:" + robot.currentNode.name);
                break;
            case 1:
                edgesList = new List<vertex>();
                stage++;
                break;
            case 2:
                foreach (node Node in nodesCovered)
                {
                    foreach (vertex edge in robot.graph.vertices)
                    {
                        if (edge.start == Node )
                        {
                            robot.log.Add("Considering edge:" + edge.name);
                            if (!nodesCovered.Contains(edge.end))
                            {
                                robot.log.Add("edge:" + edge.name + " is added to list of edges to consider");
                                edgesList.Add(edge);
                            }
                            else
                            {
                                robot.log.Add("edge:" + edge.name + " rejected as it creates loop");
                            }
                        } else if (edge.end == Node)
                        {
                            robot.log.Add("Considering edge:" + edge.name);
                            if (!(nodesCovered.Contains(edge.start) ))
                            {
                                robot.log.Add("edge:" + edge.name + " is added to list of edges to consider");
                                edgesList.Add(edge);
                            }
                            else
                            {
                                robot.log.Add("edge:" + edge.name + " rejected as it creates loop");
                            }
                        }
                    }
                }
                foreach(vertex edge in edgesList)
                {
                    edge.transform.GetComponent<GraphItemScript>().highlightSecondary();
                }
                stage++;
                break;
            case 3:

                if (edgesList.Count == 0) 
                { 
                    robot.log.Add("all nodes connected, program complete");

                    List<Vector3> allNodesPositions = new List<Vector3>();

                    foreach(node Node in robot.graph.nodes)
                    {
                        allNodesPositions.Add(Node.transform.position);
                    }

                    robot.cameraManager.includeAllObject(allNodesPositions);

                    return false; 
                
                }
                smallestEdge = null;

                foreach(vertex edge in edgesList)
                {
                    if(smallestEdge == null || edge.weight < smallestEdge.weight)
                    {
                        if (smallestEdge != null)
                        {
                            smallestEdge.script.resetColour();
                        }
                        smallestEdge = edge;
                    }
                    else
                    {
                        edge.script.resetColour();
                    }

                }

                    robot.log.Add("edge:" + smallestEdge.name + " has been added to tree");
                    smallestEdge.script.hightlightPrimary();
                    finalEdges.Add(smallestEdge);

                    robot.currentNode.transform.GetComponent<GraphItemScript>().hightlightPrimary();

                    if (nodesCovered.Contains(smallestEdge.start) ){ robot.currentNode = smallestEdge.end; nodesCovered.Add(smallestEdge.end); }
                    else { robot.currentNode = smallestEdge.start; nodesCovered.Add(smallestEdge.start); }

                    robot.robotGameobject.position = robot.currentNode.position;
                    smallestEdge.start.script.hightlightPrimary();
                    smallestEdge.end.script.hightlightPrimary();
                    stage = 1;

                List<Vector3> positionsToPan = new List<Vector3>();

                positionsToPan.Add(smallestEdge.start.transform.position);
                positionsToPan.Add(smallestEdge.end.transform.position);


                robot.cameraManager.includeAllObject(positionsToPan);
                

                break;
        }

        return true;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    prims, kruskals
}
[System.Serializable]
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
                robot.robotGameobject.GetComponent<robotControl>().changePosition(robot.currentNode.position);
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
                        if (!finalEdges.Contains(edge) && (nodesCovered.Contains(edge.start) || nodesCovered.Contains(edge.end)))
                        {
                            edgesList.Add(edge);
                            robot.log.Add("Considering edge:" + edge.name);
          
                        }
                    }
                }
                foreach (vertex edge in edgesList)
                {
                    edge.transform.GetComponent<GraphItemScript>().highlightSecondary();
                }
                stage++;
                break;
            case 3:

                smallestEdge = null;


                foreach (vertex edge in edgesList)
                {


                    if (smallestEdge == null || edge.weight < smallestEdge.weight)
                    {
                        bool createsLoop = false;
                        foreach (node Node in nodesCovered)
                        {

                            if ((edge.start == Node && nodesCovered.Contains(edge.end)) || (edge.end == Node && nodesCovered.Contains(edge.start)))
                            {
                                createsLoop = true;
                                robot.log.Add("edge:" + edge.name + " has been rejected as it creates a loop");
                            }
  
          
                        }

                        if (!createsLoop)
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
                    else
                    {
                        edge.script.resetColour();
                    }

                }

                if (smallestEdge == null)
                {
                    robot.log.Add("all nodes connected, program complete");

                    List<Vector3> allNodesPositions = new List<Vector3>();

                    foreach (node Node in robot.graph.nodes)
                    {
                        allNodesPositions.Add(Node.transform.position);
                    }

                    robot.cameraManager.includeAllObject(allNodesPositions);

                    return false;

                }



                robot.log.Add("edge:" + smallestEdge.name + " has been added to tree");
                smallestEdge.script.hightlightPrimary();
                finalEdges.Add(smallestEdge);

                robot.currentNode.transform.GetComponent<GraphItemScript>().hightlightPrimary();

                if (nodesCovered.Contains(smallestEdge.start)) { robot.currentNode = smallestEdge.end; nodesCovered.Add(smallestEdge.end); }
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
[System.Serializable]
public class kruskalsAlgorithm : RobotProgram
{
    bool lastEdgeAdded = false;
    vertex currentEdge = null;
    List<List<vertex>> seperateTrees = new List<List<vertex>>();
    List<vertex> edgesLeft = new List<vertex>();
    List<node> nodesCovered = new List<node>();

    vertex[] orderedEdgeList;

    List<vertex> edgeOptions = new List<vertex>();

    int num = 0;
    
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

                robot.robotGameobject.GetComponent<robotControl>().changePosition(robot.currentNode.position);
                seperateTrees = new List<List<vertex>>();
                edgesLeft = robot.graph.vertices;
                nodesCovered = new List<node>();
                stage++;



                orderedEdgeList = edgesLeft.OrderBy(o => o.weight).ToArray();

                break;
            case 1:
                Debug.Log(orderedEdgeList.Length);
                Debug.Log(num);
                if(currentEdge != null)
                {
                    if (!lastEdgeAdded)
                    {
                        currentEdge.script.resetColour();
                    }
                    else
                    {
                        currentEdge.script.hightlightPrimary();
                    }
                }
                if(orderedEdgeList.Length == num)
                {
                    robot.log.Add("all edges have been checked, tree complete");
                    return false;
                }

                currentEdge = orderedEdgeList[num];
                num++;
                robot.robotGameobject.position = currentEdge.transform.position;
                List<vertex> startTree = null, endTree = null;

                currentEdge.script.hightlightPrimary();
                List<Vector3> positionsToPan = new List<Vector3>();

                positionsToPan.Add(currentEdge.start.transform.position);
                positionsToPan.Add(currentEdge.end.transform.position);


                robot.cameraManager.includeAllObject(positionsToPan);

                bool valid = true;

                foreach(List<vertex> list in seperateTrees)
                {
                    bool startPresent = false, endPresent = false;

                    foreach(vertex ve in list)
                    {
                        if(ve.start == currentEdge.start || ve.end == currentEdge.start)
                        {
                            startPresent = true;
                            startTree = list;
                        } else if( ve.start == currentEdge.end || ve.end == currentEdge.end)
                        {
                            endPresent = true;
                            endTree = list;
                        }
                    }

                    if(startPresent && endPresent)
                    {
                        valid = false;
                    }
                }


                if (valid)
                {
                    if(startTree != null && endTree != null)
                    {

                        seperateTrees.Remove(startTree);
                        seperateTrees.Remove(endTree);
                        startTree.AddRange(endTree);
                        startTree.Add(currentEdge);

                        seperateTrees.Add(startTree);




                    }else if(startTree != null)
                    {
                        seperateTrees.Remove(startTree);
                        startTree.Add(currentEdge);
                        seperateTrees.Add(startTree);
                    }
                    else if(endTree != null)
                    {
                        seperateTrees.Remove(endTree);
                        endTree.Add(currentEdge);
                        seperateTrees.Add(endTree);
                    }
                    else
                    {
                        List<vertex> newTree = new List<vertex>();
                        newTree.Add(currentEdge);

                        seperateTrees.Add(newTree);
                    }

                    robot.currentNode = currentEdge.start;
                    robot.robotGameobject.GetComponent<robotControl>().changePosition(robot.currentNode.position);
                    robot.log.Add("edge" + currentEdge.name + " has been added to tree");
                    lastEdgeAdded = true;

                }
                else
                {
                    lastEdgeAdded = false;
                    currentEdge.script.highlightSecondary();
                    robot.log.Add("edge" + currentEdge.name + " has been rejected as it creates a loop");
                }
                break;
        }
        

        return true;
    }
}
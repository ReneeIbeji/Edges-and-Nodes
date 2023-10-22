using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    //objects
    Camera cam;
    
    RectTransform canvasRectTransform;


    //Graph objects prefabs

    public GameObject nodePrefab;
    public GameObject vertexPrefab;


    public Graph graph;
    public Robot robot;

    public GraphStateMachine stateMachine;
    

    public CameraMovementManager cameraMovementManager;
    public GraphUIManager graphUIManager;
    public AudioManager audioManager;

    public graphMode startGraphMode;
    public List<node> nodes = new List<node>();
    public List<vertex> vertices = new List<vertex>();

    //stats
    Vector2 mousePos2D;
    Vector3 lastmousePos;
    bool mouseDownLastFrame = false;

    [TextArea]
    public string importGraph;



    private void Start()
    {
        importGraph = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>().graphToLoad;
        startGraphMode = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>().startGraphMode;
        cam = Camera.main;
        
        GameObject.Find("Canvas").GetComponent<GraphUIManager>().graphManager = this;
        graphUIManager = GameObject.Find("Canvas").GetComponent<GraphUIManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>(); 

        canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        cameraMovementManager = GameObject.FindGameObjectWithTag("CameraMovementManager").GetComponent<CameraMovementManager>();
        lastmousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        //graph establish
        /*
        nodes.Add(new node("A", new Vector2(0, 0)));
        nodes.Add(new node("B", new Vector2(5, 0)));
        nodes.Add(new node("C", new Vector2(2.5f, 5)));


        graph = new Graph(nodes, vertices);

        graph.graphTransform = transform;
        graph.nodePrefab = nodePrefab;
        graph.vertexPrefab = vertexPrefab;
        graph.uiManager = graphUIManager;


        graph.AddVertex("A", "B");
        graph.AddVertex("B", "C");
        graph.AddVertex("C", "A");

        resetStateMachine(startGraphMode, graph, robot);

        */

        importUserGraph(importGraph);

    }

    private void Update()
    {

        graphInputCheck();
        stateMachine.update();

        stateMachine.InteractionMode.graph.scaleFactor =(((cam.orthographicSize - 5) / 5) / 1.75f ) + 1;



    }

    void resetStateMachine(graphMode mode, Graph graph, Robot robot)
    {
        graph.graphTransform = transform;
        graph.nodePrefab = nodePrefab;
        graph.vertexPrefab = vertexPrefab;
        graph.uiManager = graphUIManager;

        if (mode == graphMode.edit)
        {
            stateMachine = new GraphStateMachine(new GraphEdit(graph,robot,audioManager));
        } else if(mode == graphMode.play)
        {
            stateMachine = new GraphStateMachine(new GraphPlay(graph,robot,audioManager));   
        } else if(mode == graphMode.view)
        {
            stateMachine = new GraphStateMachine(new GraphView(graph, robot,audioManager));
        }


        stateMachine.InteractionMode.graph.displayGraph();
        stateMachine.InteractionMode.Enter();
        graphUIManager.setGraphNameBox(stateMachine.InteractionMode.graph.name);
    }



    void graphInputCheck()
    {
        if (!IsPointerOverUIElement()) {
            mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            stateMachine.InteractionMode.mousePos = mousePos2D;
            stateMachine.InteractionMode.altDown = Input.GetKey(KeyCode.AltGr);
            stateMachine.InteractionMode.mouseDown = Input.GetMouseButton(0);

            if (Input.GetMouseButtonDown(0))
            {
                stateMachine.InteractionMode.isClick = true;

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null && hit.transform.tag == "GraphItem")
                {
                    if(hit.transform.GetComponent<GraphItemScript>().type == GraphItemType.node)
                    {
                        stateMachine.InteractionMode.onClickNode(hit.transform.GetComponent<GraphItemScript>().nodeItem);
                    }
                    else if(hit.transform.GetComponent<GraphItemScript>().type == GraphItemType.vertex){
                        stateMachine.InteractionMode.onClickVertex(hit.transform.GetComponent<GraphItemScript>().vertexItem);
                    }
                    
                }
                else
                {
                    stateMachine.InteractionMode.onClickNode(null);
                }

            }
            else if (stateMachine.InteractionMode.selectedItems[0] != null && !(Input.GetKey(KeyCode.LeftAlt) || !Input.GetKey(KeyCode.RightAlt) || !Input.GetKey(KeyCode.AltGr)))
            {
                stateMachine.InteractionMode.onClickNode(null);
            }

            //move camera around
            if (!stateMachine.InteractionMode.textBoxOpen && mouseDownLastFrame && Input.GetMouseButton(1) )
            {
                stateMachine.InteractionMode.tool = selectedTool.none;
                stateMachine.InteractionMode.onClickNode(null);
                cameraMovementManager.MoveInDirection(-(Input.mousePosition - lastmousePos) * (cam.orthographicSize / 5) * canvasRectTransform.localScale.x, 2.5f);
                cameraMovementManager.movingTowardsTargetPoint = false;
                cameraMovementManager.movingTowardsTargetFOV = false;
            }

            if (stateMachine.InteractionMode.graph.graphLoaded && Input.GetKey(KeyCode.A))
            {
                List<Vector3> graphItemsPos = new List<Vector3>();

                foreach (node N in stateMachine.InteractionMode.graph.nodes)
                {
                    graphItemsPos.Add(N.transform.position);
                }

                cameraMovementManager.includeAllObject(graphItemsPos);


            }
        }


        //cameraMovement
        if (!stateMachine.InteractionMode.textBoxOpen)
        {
            if (Input.GetKey(KeyCode.O))
            {
                cameraMovementManager.ZoomOut(15 * Time.deltaTime);
                cameraMovementManager.movingTowardsTargetPoint = false;
                cameraMovementManager.movingTowardsTargetFOV = false;
            }
            if (Input.GetKey(KeyCode.P))
            {
                cameraMovementManager.ZoomOut(-15 * Time.deltaTime);
                cameraMovementManager.movingTowardsTargetPoint = false;
                cameraMovementManager.movingTowardsTargetFOV = false;
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                cameraMovementManager.ZoomOut(200 * -(Input.mouseScrollDelta.y) * Time.deltaTime);
                cameraMovementManager.movingTowardsTargetPoint = false;
                cameraMovementManager.movingTowardsTargetFOV = false;
                /*
                if (Input.mouseScrollDelta.y > 0)
                {
                    cameraMovementManager.MoveInDirection(new Vector3(mousePos2D.x - cam.transform.position.x, mousePos2D.y - cam.transform.position.y) * (cam.orthographicSize / 2.5f),5f);
                }
                */
            }
        }

        lastmousePos = Input.mousePosition;
        mouseDownLastFrame = Input.GetMouseButton(1);
    }
    public void importUserGraph(string text)
    {

        Graph newGraph = loadGraph(text);


        if (stateMachine.InteractionMode.graph.graphLoaded)
        {
            stateMachine.InteractionMode.graph.unDisplayGraph();
        }


        resetStateMachine(graphMode.edit, newGraph, robot);

    }

    public void saveUserGraph()
    {
        Graph graph = stateMachine.InteractionMode.graph;
        //string path = EditorUtility.OpenFolderPanel("Select folder to save graph to", "", "") + "/" +graph.name + ".txt";

        string text = "";

            text += graph.name + "\n";
            text += "Nodes:" + Convert.ToString(graph.nodes.Count) + "\n";
            foreach (node Node in graph.nodes)
            {
                text += (Node.name + "," + Node.position.x + "," + Node.position.y) + "\n";
            }
            text += ("Edges:" + Convert.ToString(graph.vertices.Count)) + "\n";
            foreach (vertex Edge in graph.vertices)
            {
                text += (Edge.name + "," + Edge.start.name + "," + Edge.end.name + "," + Edge.weight) + "\n";
            }

        graphUIManager.addTextToExport(text);
    }
    public Graph loadGraph(string text)
    {
        string[] file = text.Split("\n");
        int i = 0;
        List<node> nodes = new List<node>();

        string name = file[i];
        i++;

        int nodeNum = Convert.ToInt32(file[i].Split(":")[1]);
            i++;

            string line;
            for (int z = 0; z < nodeNum; z++)
            {

                line = file[i];
                i++;
                string[] nodeValues = line.Split(",");
                string nodeName = line.Split(",")[0];

                Vector3 nodePos = new Vector3(float.Parse(nodeValues[1]), float.Parse(nodeValues[2]), 0);


                nodes.Add(new node(nodeName, nodePos));
            }

            List<vertex> edges = new List<vertex>();

            int edgeNum = Convert.ToInt32(file[i].Split(":")[1]);
            i++;

            for (int z = 0; z < edgeNum; z++)
            {
                line = file[i];
                i++;
                string[] edgeValues = line.Split(",");

                string edgeName = edgeValues[0];
                node edgeStartNode = nodes.Find(x => x.name == edgeValues[1]);
                node edgeEndNode = nodes.Find(x => x.name == edgeValues[2]);
                float edgeWeight = float.Parse(edgeValues[3]);

                edges.Add(new vertex(edgeName, edgeStartNode, edgeEndNode, edgeWeight));
            }


            Graph newGraph = new Graph(name, nodes, edges);
            newGraph.graphTransform = transform;
            newGraph.nodePrefab = nodePrefab;
            newGraph.vertexPrefab = vertexPrefab;
            newGraph.uiManager = graphUIManager;

            return newGraph;


    }

    public void changeGraphName(string newName)
    {
        stateMachine.InteractionMode.graph.name = newName;
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

}

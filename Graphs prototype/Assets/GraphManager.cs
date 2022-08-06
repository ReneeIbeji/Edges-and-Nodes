using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public List<node> nodes = new List<node>();
    public List<vertex> vertices = new List<vertex>();

    //stats
    Vector2 mousePos2D;
    Vector3 lastmousePos;
    bool mouseDownLastFrame = false;



    private void Start()
    {
        //item to establish
        cam = Camera.main;

        GameObject.Find("Canvas").GetComponent<GraphUIManager>().graphManager = this;
        graphUIManager = GameObject.Find("Canvas").GetComponent<GraphUIManager>();

        canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        cameraMovementManager = GameObject.FindGameObjectWithTag("CameraMovementManager").GetComponent<CameraMovementManager>();
        lastmousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //graph establish
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
        stateMachine = new GraphStateMachine(new GraphEdit(graph,robot));

        stateMachine.InteractionMode.graph.displayGraph();
        stateMachine.InteractionMode.Enter();
    }

    private void Update()
    {

        graphInputCheck();
        stateMachine.update();

        stateMachine.InteractionMode.graph.scaleFactor =(((cam.orthographicSize - 5) / 5) / 1.75f ) + 1;



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

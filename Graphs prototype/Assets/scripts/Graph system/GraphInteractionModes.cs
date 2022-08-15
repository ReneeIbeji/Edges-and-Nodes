using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum graphMode
{
    edit,
    view, 
    play
}

public enum selectedTool
{
    node,
    vertex,
    delete,
    none
}

[System.Serializable]

public class GraphStateMachine{
    public enum state
    {
        enter,
        update,
        exit
    }

    state currentState;
    public GraphInteractionMode InteractionMode;


    public GraphStateMachine(GraphInteractionMode mode)
    {
        this.InteractionMode = mode;
    }
    public void update()
    {
        InteractionMode.Process();
    }

    public void changeState(graphMode mode)
    {
        InteractionMode.audioManager.Play("SwitchMode");
        if(mode == graphMode.edit)
        {
            transitionToNewState(new GraphEdit(InteractionMode.graph,InteractionMode.robot,InteractionMode.audioManager));
        }else if(mode == graphMode.view)
        {
            transitionToNewState(new GraphView(InteractionMode.graph,InteractionMode.robot,InteractionMode.audioManager));
        }
        else if(mode == graphMode.play)
        {
            transitionToNewState(new GraphPlay(InteractionMode.graph, InteractionMode.robot,InteractionMode.audioManager));
            transitionToNewState(new GraphPlay(InteractionMode.graph, InteractionMode.robot,InteractionMode.audioManager));
        }

    }


    void transitionToNewState(GraphInteractionMode newState)
    {
        InteractionMode.Exit();

        InteractionMode = newState;

        InteractionMode.Enter();
    }
}

[System.Serializable]
public class GraphInteractionMode
{

    //general items
    public graphMode mode;
    public GraphItem[] selectedItems = new GraphItem[2];

    public Robot robot;
    public Graph graph;
    public selectedTool tool;

    public GraphUIManager uiManager;
    public AudioManager audioManager;


    //input
    public bool mouseDown;
    public bool isClick;
    public Vector2 mousePos;
    public bool altDown;


    //state
    public bool textBoxOpen;

    public GraphInteractionMode(graphMode _mode,Graph _graph, Robot _robot, AudioManager _audioManager)
    {
        mode = _mode;
        graph = _graph;
        robot = _robot;
        audioManager = _audioManager;
        robot.graph = graph;
    }
    public virtual void Enter()
    {
        selectedItems[0] = new GraphItem();
        selectedItems[1] = new GraphItem();
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GraphUIManager>();
    }
    public virtual void Process()
    {
        if (selectedItems[0] != null && selectedItems[0].type != GraphItemType.none &&  mouseDown) { onHoldGraphItem(); }

        if (graph.graphLoaded) { graph.updateGraphPosition(); }


        mouseDown = false;
        isClick = false;
        altDown = false;
    }
    public virtual void Exit()
    {
        onClickNode(null);
        foreach(node Node in graph.nodes)
        {
            Node.script.resetColour();
        }
        foreach (vertex Vertex in graph.vertices)
        {
            Vertex.script.resetColour();
        }
    }


    public virtual void command(string Com)
    {
        
    }



    public virtual void onClickNode(node inputNode)
    {

        clearHighlight(selectedItems[0], selectedItems[1]);
        if (inputNode != null )
        {
            node item = graph.findNode(inputNode.name);

            if (selectedItems[0] != item) { 
                if (selectedItems[0] != item) { selectedItems[1] = selectedItems[0]; selectedItems[0] = item; addHighlight(selectedItems[0], selectedItems[1]); } 
                else { selectedItems[0] = new GraphItem(); } 
                Debug.Log("item has been selected");
                audioManager.Play("SelectObject");

            }
            else { selectedItems[0] = new GraphItem(); selectedItems[1] = new GraphItem(); audioManager.Play("UnSelectObject");  Debug.Log("item has been deselected"); }
        }
        else
        {
           
            selectedItems[0] = new GraphItem();
            selectedItems[1] = new GraphItem();
            Debug.Log("item has been deselected");
        }


    }

    public virtual void onClickVertex(vertex inputVertex)
    {

        clearHighlight(selectedItems[0], selectedItems[1]);
        if (inputVertex != null )
        {
            vertex item = graph.findVertex(inputVertex.start.name, inputVertex.end.name);
            if (selectedItems[0] != item) { if (selectedItems[0] != item) { selectedItems[1] = selectedItems[0]; selectedItems[0] = item; addHighlight(selectedItems[0], selectedItems[1]); }
                else { selectedItems[0] = new GraphItem(); } 
                Debug.Log("item has been selected");
                audioManager.Play("SelectObject");
            }
            else { selectedItems[0] = new GraphItem(); selectedItems[1] = new GraphItem(); audioManager.Play("UnSelectObject"); Debug.Log("item has been deselected");  }
        }
        else
        {
            selectedItems[0] = new GraphItem();
            selectedItems[1] = new GraphItem();
            Debug.Log("item has been deselected");
        }



    }

    public virtual void onHoldGraphItem()
    {
        Debug.Log("selcted item is being held");
    }

     void addHighlight(GraphItem item1,GraphItem item2)
     {
        if (item1.type != GraphItemType.none)
        {
            item1.script.hightlightPrimary();
        }
        if (item2.type != GraphItemType.none)
        {
            item2.script.highlightSecondary();
        }
     }
    void clearHighlight(GraphItem item1, GraphItem item2)
    {
        if (item1.type != GraphItemType.none)
        {
            item1.script.resetColour();
        }
        if (item2.type != GraphItemType.none)
        {
            item2.script.resetColour();
        }
    }

    public void changeNodeName(node item, string newName)
    {
        graph.changeNodeName(item, newName);
    }
    public void changeVertexName(vertex item, string newName)
    {
        graph.changeVertexName(item, newName);
    }

    public void changeVertexWeight(vertex item, int newWeight)
    {
        graph.changeVertexWeight(item, newWeight);
    }




}

public class GraphEdit : GraphInteractionMode
{


    public GraphEdit(Graph graph,Robot robot,AudioManager audioManager)
    : base(graphMode.edit, graph,robot,audioManager)
    {
        tool = selectedTool.none;
    }

    public override void Process()
    {
        if (tool == selectedTool.none)
        {
            base.Process();
        }
        else
        {

            if (graph.graphLoaded) { graph.updateGraphPosition(); }
        }


        if (tool != selectedTool.none && isClick)
        {
            useTool();
        }



            mouseDown = false;
            isClick = false;
    }
    public override void command(string Com)
    {
        Debug.Log(Com);
        if (Com.Split("/")[0] == "changeTool")
        {
            if (Com.Split("/")[1] == "Node")
            {
                tool = selectedTool.node;
            } else if (Com.Split("/")[1] == "Vertex") {
                tool = selectedTool.vertex;
                onClickNode(null);
            } else if (Com.Split("/")[1] == "Delete")
            {
                onClickNode(null);
                tool = selectedTool.delete;
                
            }
            
            else if (Com.Split("/")[1] == "None")
            {
                tool = selectedTool.none;
            }
        }
        else if(Com == "addNode")
        {
            addNode();
        }
        base.command(Com);
    }


    public override void onHoldGraphItem()
    {
        if (selectedItems[0].type == GraphItemType.node)
        {
            graph.updateNodePosition(selectedItems[0], mousePos);
        }

        base.onHoldGraphItem();
    }

    //commands

    void addNode()
    {
        onClickNode(null);
        onClickNode( graph.AddNode("newNode",mousePos));

         
    }

    void addVertex(string nodeA, string nodeB)
    {
        onClickVertex(null);
        onClickVertex(graph.AddVertex(nodeA, nodeB));
    }

    void deleteNode(string nodeName)
    {
        onClickNode(null);
        graph.DeleteNode(nodeName);
    }
    
    void deleteVertex(string startNode,string endNode)
    {
        onClickNode(null);
        graph.DeleteVertex(startNode, endNode);
    }


    void useTool()
    {
        Debug.Log("using:" + tool.ToString());
        if(tool == selectedTool.node )
        {
            onClickNode(null);
            addNode();
            tool = selectedTool.none;
        } 
        if(tool == selectedTool.vertex)
        {
            if (selectedItems[0] != null && selectedItems[1] != null && selectedItems[0].type == GraphItemType.node && selectedItems[1].type == GraphItemType.node)
            {
                addVertex(selectedItems[0].name, selectedItems[1].name);
                tool = selectedTool.none;
            }
        }
        if(tool == selectedTool.delete)
        {
            if (selectedItems[0] != null)
            {
                if (selectedItems[0].type == GraphItemType.node)
                {
                    deleteNode(selectedItems[0].name);
                    onClickNode(null);
                } 
                else if (selectedItems[0].type == GraphItemType.vertex)
                {

                    deleteVertex(selectedItems[0].name.Split("-")[0], selectedItems[0].name.Split("-")[1]);

                }

                tool = selectedTool.none;
            }
        }

        
    }
}

public class GraphView: GraphInteractionMode
{
    public GraphView(Graph graph,Robot robot,AudioManager audioManager)
    : base(graphMode.view, graph,robot,audioManager)
    {
        tool = selectedTool.none;
    }

    public override void Enter()
    {
        base.Enter();
        robot.graph = graph;
    }
    public override void Process()
    {
        base.Process();
    }

    public override void command(string Com)
    {
        base.command(Com);
    }


    public override void onHoldGraphItem()
    {
        base.onHoldGraphItem();
    }


    //commands

}

public class GraphPlay : GraphInteractionMode
{
    public GraphPlay(Graph graph, Robot robot,AudioManager audioManager)
    : base(graphMode.play, graph, robot,audioManager)
    {
        tool = selectedTool.none;
    }

    public override void Enter()
    {
        base.Enter();
        uiManager.putAllNodesOnDropDown();
        robot.graph = graph;
        robot.robotScript = robot.robotGameobject.GetComponent<robotControl>();
        uiManager.changeRobotStartNode();
        uiManager.changeRobotProgram();
        robot.robotScript.show();
        robot.audioManager = audioManager;
    }
    public override void Process()
    {
        if (robot.programInProgress)
        {
            robot.carryOutProgram();
            
        }

        if (graph.graphLoaded) { graph.updateGraphPosition(); }
    }

    public override void Exit()
    {
        robot.robotScript.hide();
        robot.log.Clear();
        robot.endProgram();
        uiManager.updateRobotLog();
        base.Exit();
    }
    public override void onClickNode(node inputNode)
    {

        if (inputNode != null)
        {
            node item = graph.findNode(inputNode.name);

            if (selectedItems[0] != item)
            {
                if (selectedItems[0] != item) { selectedItems[1] = selectedItems[0]; selectedItems[0] = item; }
                else { selectedItems[0] = new GraphItem(); }
                Debug.Log("item has been selected");
            }
            else { selectedItems[0] = new GraphItem(); selectedItems[1] = new GraphItem(); Debug.Log("item has been deselected"); }
        }
        else
        {

            selectedItems[0] = new GraphItem();
            selectedItems[1] = new GraphItem();
            Debug.Log("item has been deselected");
        }


    }

    public override void onClickVertex(vertex inputVertex)
    {

        if (inputVertex != null)
        {
            vertex item = graph.findVertex(inputVertex.start.name, inputVertex.end.name);
            if (selectedItems[0] != item)
            {
                if (selectedItems[0] != item) { selectedItems[1] = selectedItems[0]; selectedItems[0] = item; }
                else { selectedItems[0] = new GraphItem(); }
                Debug.Log("item has been selected");
            }
            else { selectedItems[0] = new GraphItem(); selectedItems[1] = new GraphItem(); Debug.Log("item has been deselected"); }
        }
        else
        {
            selectedItems[0] = new GraphItem();
            selectedItems[1] = new GraphItem();
            Debug.Log("item has been deselected");
        }
    }




    public override void command(string Com)
    {

        Debug.Log(Com);
        if (Com == "startProgram")
        {
            robot.startProgram();
        }
        else if (Com == "pauseProgram")
        {
            robot.pauseProgram();
        }
        else if (Com == "continueProgram")
        {
            robot.unPauseProgram();
        }
    }


    public override void onHoldGraphItem()
    {
        base.onHoldGraphItem();
    }
}


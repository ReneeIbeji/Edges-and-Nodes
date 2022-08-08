using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using TMPro;

public class GraphUIManager : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject contentPanel;


    public GameObject itemTemplate;
    public GameObject edgeWeightTextPrefab;

    public bool toolIsSelected;
    public GameObject selectedToolOption;
    public TMP_InputField robotLog;

    bool itemIsSelected = false;
    bool justChangedObeject = false;
    GraphItem selectedItem;

    public TMP_InputField graphNameInput;


    public GameObject NodeAttributeMenu;
    public TMP_InputField NodeAttributeName;

    public GameObject EdgeAttributeMenu;
    public TMP_InputField EdgeAttributeName;
    public TMP_InputField EdgeAttributeWeight;

    public GameObject weightsParent;

    GameObject mouseItem;
    RectTransform mouseItemRect;

    Canvas myCanvas;
    RectTransform canvasRectTransform;


    [System.Serializable]
    public struct modeUI
    {
        public GameObject display;
        public graphMode mode;
    }

    [System.Serializable]
    public struct contentGraphItem
    {
        public selectedTool type;
        public Sprite sprite;
        public string name;
        public Sprite toolSignifier;
    }

    public contentGraphItem[] toolBoxItems;
    public modeUI[] modeDisplays;

    public TMP_Dropdown startNodeDropDown;

    public GameObject exportPanel;
    public TMP_InputField exportInputField;

    public GameObject importPanel;
    public TMP_InputField importInputField;



  

    void Start()
    {
        canvasRectTransform = gameObject.GetComponent<RectTransform>();
        int shift = -100;
        foreach (var item in toolBoxItems)
        {
            GameObject currentsprite = Instantiate(itemTemplate, contentPanel.transform.position, Quaternion.identity);

            currentsprite.transform.SetParent(contentPanel.transform);
 
            currentsprite.GetComponent<Image>().sprite = item.sprite;

            currentsprite.GetComponent<RectTransform>().position = contentPanel.GetComponent<RectTransform>().position + new Vector3(shift * (canvasRectTransform.position.x / 481), 0,0);
            currentsprite.GetComponent<RectTransform>().localScale *= (canvasRectTransform.position.x / 481);

            currentsprite.GetComponent<toolBoxItem>().itemName = item.name;
            currentsprite.GetComponent<toolBoxItem>().graphUIManager = this;

            shift += 100;
        }

        mouseItem = Instantiate(itemTemplate, contentPanel.transform.position, Quaternion.identity);
        mouseItem.transform.SetParent(contentPanel.transform.parent);

        Destroy(mouseItem.GetComponent<toolBoxItem>());
        Destroy(mouseItem.GetComponent<Button>());

        mouseItem.GetComponent<Image>().raycastTarget = false;

        mouseItem.GetComponent<Image>().enabled = false;

        myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        mouseItemRect = mouseItem.GetComponent<RectTransform>();

    }

    private void Update()
    {
        justChangedObeject = false;
        if (toolIsSelected == true && graphManager.stateMachine.InteractionMode.tool == selectedTool.none)
        {
            toolIsSelected = false;
            selectedToolOption.GetComponent<toolBoxItem>().selected = false;
            selectedToolOption = null;
            mouseItem.GetComponent<Image>().enabled = false;
        }

        foreach(modeUI item in modeDisplays)
        {
            if(item.mode == graphManager.stateMachine.InteractionMode.mode) { item.display.SetActive(true); }
            else { item.display.SetActive(false); }
        }

        if ( !itemIsSelected && graphManager.stateMachine.InteractionMode.selectedItems[0].type != GraphItemType.none)
        {
            NodeAttributeMenu.SetActive(false);
            EdgeAttributeMenu.SetActive(false);
            selectedItem = graphManager.stateMachine.InteractionMode.selectedItems[0];
            if (selectedItem.type == GraphItemType.node)
            {
                NodeAttributeMenu.SetActive(true);
                NodeAttributeName.text = graphManager.stateMachine.InteractionMode.selectedItems[0].name;
            } else if (selectedItem.type == GraphItemType.vertex)
            {
                EdgeAttributeMenu.SetActive(true);
                EdgeAttributeName.text = graphManager.stateMachine.InteractionMode.selectedItems[0].name;
                EdgeAttributeWeight.text = Convert.ToString( graphManager.stateMachine.InteractionMode.selectedItems[0].script.vertexItem.weight);
            }
            selectedItem = graphManager.stateMachine.InteractionMode.selectedItems[0];
            itemIsSelected = true;

        }
        else if(itemIsSelected && graphManager.stateMachine.InteractionMode.selectedItems[0].type == GraphItemType.none)
        {
            NodeAttributeMenu.SetActive(false);
            EdgeAttributeMenu.SetActive(false);
            justChangedObeject = true;
            selectedItem = graphManager.stateMachine.InteractionMode.selectedItems[0];
            itemIsSelected = false;
        }
        else if (itemIsSelected && graphManager.stateMachine.InteractionMode.selectedItems[0] != selectedItem)
        {
            justChangedObeject = true;
            NodeAttributeMenu.SetActive(false);
            EdgeAttributeMenu.SetActive(false);
            selectedItem = graphManager.stateMachine.InteractionMode.selectedItems[0];
            if (selectedItem.type == GraphItemType.node)
            {
                NodeAttributeMenu.SetActive(true);
                NodeAttributeName.text = graphManager.stateMachine.InteractionMode.selectedItems[0].name;
            }
            else if (selectedItem.type == GraphItemType.vertex)
            {
                EdgeAttributeMenu.SetActive(true);
                EdgeAttributeName.text = graphManager.stateMachine.InteractionMode.selectedItems[0].name;
                EdgeAttributeWeight.text = Convert.ToString(graphManager.stateMachine.InteractionMode.selectedItems[0].script.vertexItem.weight);
            }
 
            selectedItem = graphManager.stateMachine.InteractionMode.selectedItems[0];
        }



        if (graphManager.stateMachine.InteractionMode.mode == graphMode.play && graphManager.stateMachine.InteractionMode.robot.needToUpdateLog)
        {
            updateRobotLog();
            graphManager.stateMachine.InteractionMode.robot.needToUpdateLog = false;
        }


        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        mouseItemRect.position = myCanvas.transform.TransformPoint(pos) + new Vector3(50,50,0);

        
    }

    public vertex addWeightLabel(vertex edge)
    {

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(edge.transform.position);
            Vector3 proportionalPosition = new Vector2(ViewportPosition.x * canvasRectTransform.sizeDelta.x * canvasRectTransform.localScale.x, ViewportPosition.y * canvasRectTransform.sizeDelta.y * canvasRectTransform.localScale.y);
            proportionalPosition.z = -5;
            GameObject label =Instantiate(edgeWeightTextPrefab, Vector2.zero, Quaternion.identity);
            label.GetComponent<RectTransform>().position = proportionalPosition;
            label.name = "weight label";
            label.transform.SetParent(weightsParent.transform);
            label.transform.localScale *= (canvasRectTransform.position.x / 481);

        edge.weightText = label.GetComponent<TMP_Text>();
            edge.weightText.text = Convert.ToString( edge.weight);
            

        return edge;
    }

    public void updateWeightLabel(vertex edge)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(edge.transform.position);
        Vector3 proportionalPosition = new Vector2(ViewportPosition.x * canvasRectTransform.sizeDelta.x * canvasRectTransform.localScale.x, ViewportPosition.y * canvasRectTransform.sizeDelta.y * canvasRectTransform.localScale.y);
        proportionalPosition.z = -5;
        edge.weightText.rectTransform.position = proportionalPosition;
        edge.weightText.text = Convert.ToString(edge.weight);

    }

    public void switchToPlayMode()
    {
        graphManager.stateMachine.changeState(graphMode.play);
    }
    public void switchToEditMode()
    {
        graphManager.stateMachine.changeState(graphMode.edit);
    }
    public void switchToViewMode()
    {
        graphManager.stateMachine.changeState(graphMode.view);
    }


    public void OnToolBoxItemClicked(string name,GameObject item)
    {
        graphManager.stateMachine.InteractionMode.command("changeTool/" + name);
        if(toolIsSelected)
        {
            selectedToolOption.GetComponent<toolBoxItem>().selected = false;
            selectedToolOption = item;
        }
        else
        {
            selectedToolOption = item;
            toolIsSelected = true;
            mouseItem.GetComponent<Image>().enabled = true;
        }

    }


    //edit mode buttons;

    public void onOpenField()
    {
        graphManager.stateMachine.InteractionMode.textBoxOpen = true;
    }
    public void onExitField()
    {
        graphManager.stateMachine.InteractionMode.textBoxOpen = false;
    }


    public void onEditNodeNameField()
    {
        if (!justChangedObeject)
        {
            graphManager.stateMachine.InteractionMode.changeNodeName(selectedItem.transform.GetComponent<GraphItemScript>().nodeItem, NodeAttributeName.text);


        }
    }   
    
    public void onEditEdgeNameField()
    {
        if (!justChangedObeject)
        {
            graphManager.stateMachine.InteractionMode.changeVertexName(selectedItem.transform.GetComponent<GraphItemScript>().vertexItem, EdgeAttributeName.text);
            

        }
    }

    public void onEditVertexWeightField()
    {
        if (!justChangedObeject)
        {
            graphManager.stateMachine.InteractionMode.changeVertexWeight(selectedItem.transform.GetComponent<GraphItemScript>().vertexItem,Convert.ToInt32( EdgeAttributeWeight.text));


        }
    }

    public void setGraphNameBox(string name)
    {
        graphNameInput.text = name;
    }

    public void onEditGraphNameBox()
    {
        graphManager.changeGraphName(graphNameInput.text);
    }

    //import and export graph


    public void displayExportPanel()
    {
        graphManager.saveUserGraph();
        exportPanel.SetActive(true);

    }
    public void hideDisplayExportPanel()
    {
        exportPanel.SetActive(false);
    }

    public void addTextToExport(string text)
    {
        exportInputField.text = text;
    }

    public void saveGraph()
    {
        graphManager.saveUserGraph();
    }




    public void displayImportPanel()
    {
        importPanel.SetActive(true);
    }
    public void hideDisplayImportPanel()
    {
        importPanel.SetActive(false);
    }

    public void importUserGraph()
    {
        graphManager.importUserGraph(importInputField.text);
        hideDisplayImportPanel();
    }

    //robot stuff

    //start robot program

    public void startProgram()
    {
        graphManager.stateMachine.InteractionMode.command("startProgram");
    }


    //pause robot program
    public void pauseProgram()
    {
        graphManager.stateMachine.InteractionMode.command("pauseProgram");
    }

    //unpause robot program
    public void unPauseProgram()
    {
        graphManager.stateMachine.InteractionMode.command("continueProgram");
    }

    // robot log

    public void updateRobotLog()
    {
        robotLog.text = "";
        int i = 1;
            
        foreach(string n in graphManager.stateMachine.InteractionMode.robot.log.ToArray())
        {
            robotLog.text += Convert.ToString(i)+ "." + n + "\n" + "__________" + "\n";
            i++;
        }
        robotLog.verticalScrollbar.value = 1;
    }



    //robot settings

    public void putAllNodesOnDropDown()
    {
        startNodeDropDown.ClearOptions();
        foreach(node Node in graphManager.stateMachine.InteractionMode.graph.nodes)
        {
            startNodeDropDown.AddOptions(new List<string> { Node.name});
        }

        changeRobotStartNode();
    }

    public void changeRobotStartNode()
    {
        if (!graphManager.stateMachine.InteractionMode.robot.programInProgress && !graphManager.stateMachine.InteractionMode.robot.programPaused)
        {
            if (graphManager.stateMachine.InteractionMode.robot.startNode.script != null)
            {
                graphManager.stateMachine.InteractionMode.robot.startNode.script.resetColour();
            }

            Debug.Log("Noded assigned");
            node assignNode = graphManager.stateMachine.InteractionMode.graph.findNode(startNodeDropDown.options[startNodeDropDown.value].text);
            graphManager.stateMachine.InteractionMode.robot.startNode = assignNode;
            assignNode.script.hightlightPrimary();
            graphManager.cameraMovementManager.includeAllObject(new List<Vector3> { assignNode.transform.position });
        }

    }
}
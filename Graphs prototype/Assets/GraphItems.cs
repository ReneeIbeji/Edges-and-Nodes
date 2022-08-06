using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GraphItemType
{
	node,
	vertex,
	none
}

[System.Serializable]
public class Graph
{
	public string name;

	public bool graphLoaded = false;

	public List<node> nodes;
	public List<vertex> vertices;

	public Transform graphTransform;
	public GraphUIManager uiManager;
	public GameObject nodePrefab;
	public GameObject vertexPrefab;
	public float scaleFactor;

    public Graph(string _name, List<node> _nodes, List<vertex> _vertices)
    {
		name = _name;
		nodes = _nodes;
		vertices = _vertices;
    }

	public Graph(List<node> _nodes, List<vertex> _vertices)
    {
		nodes = _nodes;
		vertices = _vertices;
	}


	


	public node findNode(string name)
    {
		return nodes.Find(x => x.name == name);
	}
	
	public vertex findVertex(string start, string end)
    {
		vertex result = null;
		foreach(vertex Vertex in vertices)
        {
			if(Vertex.start.name == start && Vertex.end.name == end)
            {
				result = Vertex;
            }
        }
		return result;
	}

	public node AddNode(string name, Vector2 pos)
    {
		string nodeName = name;
		int num = 1;
		while(nodes.Exists(x => x.name == nodeName))
        {
			nodeName = name + "_" + num;
			num++;
        }

		node Newnode = new node(nodeName, pos) ;
		nodes.Add(Newnode);
		if (graphLoaded)
		{
			Newnode = loadNewNode(Newnode);
		}

		return Newnode;

		
    }
	public vertex AddVertex(string _start,string _end)
    {
		node nodeA = findNode(_start);
		node nodeB = findNode(_end);
		vertex newVertex = new vertex();

		vertex dupVertex;

		foreach(vertex Ve in vertices)
        {
			if(Ve.start.name == _start && Ve.end.name == _end)
            {
				dupVertex = Ve;
				Debug.Log("AddVertex: node " + _start + " or node " + _end + " already exists");
				return dupVertex;
            }
        }






		if (nodeA != null && nodeB != null) {
			 newVertex = new vertex(nodeA, nodeB);
			vertices.Add(newVertex);
			if (graphLoaded) { newVertex = loadNewVertex(newVertex); newVertex = uiManager.addWeightLabel(newVertex);  } 
		}
		else { Debug.LogError("AddVertex: node " + _start + " or node " + _end + " doesn't exist"); }

		return newVertex;
	
    }

	public bool DeleteNode(string name)
    {
		if (nodes.Exists(x => x.name == name))
		{
			node nodeToDelete = findNode(name);

			
			GameObject.Destroy(nodeToDelete.transform.gameObject);

			nodes.Remove(nodeToDelete);

			List<vertex> vertexRemoveList = new List<vertex>();

			foreach (vertex Vertex in vertices)
			{
				if (Vertex.start.name == name || Vertex.end.name == name)
				{
					vertexRemoveList.Add(Vertex);
				}
			}

			foreach(vertex item in vertexRemoveList)
            {
				DeleteVertex(item.start.name, item.end.name);
            }

			return true;
        }
        else { return false; }
    }


	public bool DeleteVertex(string start,string end)
    {
		vertex Ve = findVertex(start,end);

		if(Ve != null)
        {
			GameObject.Destroy(Ve.weightText.gameObject);
			GameObject.Destroy(Ve.transform.gameObject);

			
			
			vertices.Remove(Ve);

			return true;
        }
        else
        {
			return false;
        }
    }

	public void changeNodeName(node item, string name)
    {
		if (!nodes.Exists(x => x.name == name))
		{
			node actualNode = findNode(item.name);
			string originalName = actualNode.name;
			nodes.Remove(actualNode);
			actualNode.name = name;

		
			nodes.Add(actualNode);

			actualNode.transform.GetComponent<GraphItemScript>().nodeItem = actualNode;

			List<vertex> vertexStartChangeList = new List<vertex>();
			List<vertex> vertexEndChangeList = new List<vertex>();


		
			foreach (vertex Vertex in vertices)
			{
				if (Vertex.start.name == originalName )
				{
					vertexStartChangeList.Add(Vertex);
				} else if (Vertex.end.name == originalName)
				{
					vertexEndChangeList.Add(Vertex);
				}
			}



			foreach(vertex Vertex in vertexStartChangeList)
			{
				vertices.Remove(Vertex);

				vertex newVertex = Vertex;
				newVertex.start.name = name;

				vertices.Add(newVertex);
				newVertex.transform.GetComponent<GraphItemScript>().vertexItem = newVertex;
			}

			foreach(vertex Vertex in vertexEndChangeList)
			{
				vertices.Remove(Vertex);

				vertex newVertex = Vertex;
				newVertex.end.name = name;
				vertices.Add(newVertex);

				newVertex.transform.GetComponent<GraphItemScript>().vertexItem = newVertex;
			}
		}
	}

	public void changeVertexName(vertex item, string name)
    {
		if (!vertices.Exists(x => x.name == name))
		{
			vertex actualVertex = findVertex(item.start.name, item.end.name);
			string originalName = actualVertex.name;
			vertices.Remove(actualVertex);
			actualVertex.name = name;

			vertices.Add(actualVertex);
		}
    }

	public void changeVertexWeight(vertex item, int weight)
    {
		vertex actualVertex = findVertex(item.start.name, item.end.name);
		vertices.Remove(actualVertex);
		actualVertex.weight = weight;

		vertices.Add(actualVertex);
	}


	public void updateNodePosition(GraphItem refNode, Vector2 newPos)
	{
		node node = findNode(refNode.name);

		if(node != null) { node.setPosition(newPos); }

		foreach (vertex Vertex in vertices)
        {
			if(Vertex.start.name == node.name)
            {
				Vertex.start = node;
            }
			if(Vertex.end.name == node.name)
            {
				Vertex.end = node;
            }
        }

		updateGraphPosition();
	}


	public void displayGraph()
    {
		foreach (node Node in nodes)
		{
			Node.transform = loadNewNode(Node).transform;


		}

		foreach (vertex Vertex in vertices)
		{
			Vertex.transform = loadNewVertex(Vertex).transform;
			Vertex.weightText = uiManager.addWeightLabel(Vertex).weightText;
		}

		
		graphLoaded = true;
	}

	node loadNewNode(node Node)
	{
		GameObject instantObject;

		instantObject = GameObject.Instantiate(nodePrefab, Node.position, Quaternion.identity);

		

		instantObject.name = "Node:" + Node.name;
		instantObject.transform.parent = graphTransform;
		Node.transform = instantObject.transform;

		instantObject.GetComponent<GraphItemScript>().nodeItem = Node;
		instantObject.GetComponent<GraphItemScript>().type = GraphItemType.node;
		instantObject.transform.localScale = new Vector3(5 * scaleFactor, 5 * scaleFactor, 1);

		Node.script = Node.transform.GetComponent<GraphItemScript>();

		return Node;
	}

	vertex loadNewVertex(vertex Vertex)
	{
		GameObject instantObject = GameObject.Instantiate(vertexPrefab, (Vertex.start.position + Vertex.end.position) / 2, Quaternion.identity);
		instantObject.GetComponent<SpriteRenderer>().size = new Vector2(0.5f * scaleFactor, Vector2.Distance(Vertex.start.position, Vertex.end.position) -0.25f );

		Vector2 target;
		target.x = Vertex.start.position.x - instantObject.transform.position.x;
		target.y = Vertex.start.position.y - instantObject.transform.position.y;
		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg + 90;
		instantObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


		instantObject.name = "Vertex:" + Vertex.name;
		instantObject.transform.parent = graphTransform;
		Vertex.transform = instantObject.transform;

		instantObject.GetComponent<GraphItemScript>().vertexItem = Vertex;
		instantObject.GetComponent<GraphItemScript>().type = GraphItemType.vertex;
		Vertex.script = Vertex.transform.GetComponent<GraphItemScript>();

		return Vertex;
	}


	public void updateGraphPosition()
	{
		foreach (node Node in nodes)
		{
			Node.transform.position = Node.position;
			Node.transform.GetComponent<GraphItemScript>().nodeItem = Node;
			Node.transform.GetComponent<GraphItemScript>().type = GraphItemType.node;
			Node.transform.localScale = new Vector3(5 * scaleFactor, 5 * scaleFactor, 1);
		}

		foreach (vertex Vertex in vertices)
		{
			Vertex.transform.position = (Vertex.start.position + Vertex.end.position) / 2;
			Vertex.transform.GetComponent<SpriteRenderer>().size = new Vector2(0.5f * scaleFactor, Vector2.Distance(Vertex.start.position, Vertex.end.position) - 0.25f) ;

			Vector2 target;
			target.x = Vertex.start.position.x - Vertex.transform.position.x;
			target.y = Vertex.start.position.y - Vertex.transform.position.y;
			float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;

			Vertex.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

			Vertex.transform.GetComponent<GraphItemScript>().vertexItem = Vertex;
			Vertex.transform.GetComponent<GraphItemScript>().type = GraphItemType.vertex;

			uiManager.updateWeightLabel(Vertex);
		}
	}


}

[System.Serializable]
public class GraphItem
{
	public Transform transform;

	public GraphItemScript script;

	public string name;
	public GraphItemType type;

	public List<string> tags = new List<string>();
	public GraphItem(string _name,GraphItemType _type)
	{
		name = _name;
		type = _type;
		

	}

	public GraphItem()
    {
		type = GraphItemType.none;
		name = "new graph";
    }

	public virtual void setPosition(Vector2 pos)
    {

    }
}

[System.Serializable]
public class node : GraphItem
{
	public Vector2 position { get; set; }
	
	public node(string _name,Vector2 _position)
	:base ( _name,GraphItemType.node)
	{
		position = _position;
	}

	public node(Vector2 _position)
	:base("", GraphItemType.node)
    {
		position = _position;
    }


	public override void setPosition(Vector2 pos)
    {
		transform.position = pos;
		position = pos;
    }
	

}

[System.Serializable]

public class vertex : GraphItem
{
	public node start;
	public node end;
	public int weight;

	public TMPro.TMP_Text weightText;

	public vertex(string _name, node _start, node _end)
	: base(_name,GraphItemType.vertex)
	{
		start = _start;
		end = _end;
		
	}

	public vertex(node _start, node _end)
	:base(_start.name + "-" + _end.name,GraphItemType.vertex)
    {
		start = _start;
		end = _end;
	}

	public vertex()
    {

    }

	public void setStartNode(node _start) { start = _start; }
	public void setEndNode(node _end) { end = _end; }
	public void setStartAndEnd(node _start, node _end) { start = _start; end = _end; }


}


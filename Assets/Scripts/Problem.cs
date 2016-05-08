using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Board;
using System;

public class Problem
{
    private Cell[,] _matrix;
    public State initialState;
    private Vector2 _finalPosition;
    private GuiManager _guimanager;

	//Initialize problem
	public Problem(Vector2 InitialPosition,Vector2 finalPosition,Cell[,] matrix,GuiManager guiManager){

		//Set initialState
		initialState = new State (InitialPosition);
        _finalPosition = finalPosition;
        _matrix = matrix;
        _guimanager = guiManager;
	}
	//Goal test
	public bool GoalTest(State state)
	{
        _guimanager.MarkThinkCell(state.GetPosition());
		return state.GetPosition() == _finalPosition;
	}

	//Path cost
	public int PathCost(State origin,Vector2 action,State dest)
	{
		return 1;
	}

	//Heuristic
	public int HeurTree(State dest)
	{
        return 0;
	}

	//Heuristic
	public int HeurGraph(State dest)
	{
        return 0;
	}

	//Utility function
	public double Utility(State dest)
	{
        return 0;
	}

	//Successor
	public List<KeyValuePair<Vector2,State>> Successor(State state)
	{
		//Next states
		List<KeyValuePair<Vector2,State>> nextStates = new List<KeyValuePair<Vector2, State>> ();

        //Get all theoretically valid movements
        List<Vector2> totalMovs = new List<Vector2>() { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
		//Create a list with valid movements
		List<Vector2> validMovs = new List<Vector2> ();

        //Get state position
        Vector2 actorPos = state.GetPosition();
		//Loop movements to validate using unity collisions
		foreach (Vector2 mov in totalMovs) {
            //If the movement is valid is moved to valid movements
            Vector2 newPos = state.GetPosition() + mov;
            int newX = Convert.ToInt32(newPos.x);
            int newY = Convert.ToInt32(newPos.y);
            if(newX >= 0 && newX < _matrix.GetLength(0) && newY >= 0 && newY < _matrix.GetLength(1))
            if (_matrix[newX, newY].overFloor != OverFloorType.Wall)
            {
                validMovs.Add(mov);
            }
		}

		//Loop valid movements to check result
		foreach (Vector2 validMov in validMovs) {

			//Set next character state
			Vector2 nextActorPos = actorPos + validMov;
			nextStates.Add(new KeyValuePair<Vector2,State>(validMov,new State(nextActorPos)));
		}

		return nextStates;
	}

}
/// <summary>
/// This class represents a state with position for pacman, ghosts and dots
/// </summary>
public class State
{
	public string idState;
    //Vector2 pacman;
    Vector2 _position;

	public State(Vector2 position)
	{
        _position = position;
        idState = _position.x.ToString() + _position.y.ToString();
	}

    public Vector2 GetPosition()
    {
        return _position;
    }

}


/// <summary>
///This class represent a node with a state and relation to parent and action coming from
/// </summary>
public class Node
{
	public State State;
	Node Parent;
	public Vector2? Action {get;set;}
	public int Cost;
	public int CostAcumulated;
	public int HeurTree;
	public int CostHeurTree{
		get{ return this.Cost + this.HeurTree;}
	}
	public int CostAcumulatedHeurTree{
		get{ return this.CostAcumulated + this.HeurTree;}
	}
	public int HeurGraph;
	public int CostHeurGraph{
		get{ return this.Cost + this.HeurGraph;}
	}
	public int CostAcumulatedHeurGraph{
		get{ return this.CostAcumulated + this.HeurGraph;}
	}
	public double Utility;
	public int Depth;

	/// <summary>
	/// Initializes a new instance of the <see cref="Node"/> class.
	/// </summary>
	/// <param name="state">State to be attached to node</param>
	/// <param name="Parent">Parent node of this node</param>
	/// <param name="action">Action vector2 direction that generated the node</param>
	/// <param name="cost">Cost of execute action from parent node</param>
	public Node(State state,Node parent,Vector2? action,int cost,int heurTree=0,int heurGraph=0,double utility=0){
		this.State = state;
		this.Parent = parent;
		this.Action = action;
		this.Cost = cost;
		this.CostAcumulated = cost + (parent == null ? 0 : parent.CostAcumulated);
		this.HeurTree = heurTree;
		this.HeurGraph = heurGraph;
		this.Utility = utility;
		//this.CostHeur = this.Cost + this.Heur;
		if (parent != null)
			this.Depth = parent.Depth + 1;
		else
			this.Depth = 0;
	}

	/// <summary>
	/// Path this to root.
	/// </summary>
	public List<Node> Path()
	{
		List<Node> path = new List<Node> ();
		Node node = this;
		path.Add (node);
		while (node.Parent != null) {
			path.Add(node.Parent);
			node=node.Parent;
		}
		return path;
	}

	/// <summary>
	/// Return nodes after expanding
	/// </summary>
	/// <param name="prob">Problem that generates successors</param>
	public List<Node> Expand(Problem prob)
	{
		List<Node> childNodes = new List<Node> ();
		//Get successors for this state
		State origin = this.State;
		List<KeyValuePair<Vector2,State>> successors = prob.Successor (origin);
		//Create a node with each posibility
		foreach (var successor in successors) {
			Vector2 action = successor.Key;
			State destination = successor.Value;
			int cost = prob.PathCost(this.State,successor.Key,successor.Value);
			int heurtree = prob.HeurTree(destination);
			int heurgraph = prob.HeurGraph(destination);
			double utility = prob.Utility(destination);
			//Node parent = this;

			childNodes.Add(new Node(destination,this,action,cost,heurtree,heurgraph,utility));
		}

		return childNodes;
	}
	
}


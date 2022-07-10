using System.Collections.Generic;
using UnityEngine;

public static class AStar 
{ 
	public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
	{
		// Adjust positions by lower bounds
		startGridPosition -= (Vector3Int)room.templateLowerBounds;

		endGridPosition -= (Vector3Int)room.templateLowerBounds;

		// Create open list and closed hash set
		List<Node> openNodeList = new List<Node>();

		HashSet<Node> closedNodeHashSet = new HashSet<Node>();

		// Create grid nodes for path finding
		GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

		Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);

		Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

		Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instanciatedRoom);

		if (endPathNode != null)
		{
			return CreatePathStack(endPathNode, room);
		}

		return null;
	}

	private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
	{
		// Add start node to open list
		openNodeList.Add(startNode);

		// Loop through open node list until empty
		while (openNodeList.Count > 0)
		{
			// Sort list
			openNodeList.Sort();

			// current node = the node in the open list with the lowest FCost
			Node currentNode = openNodeList[0];

			openNodeList.RemoveAt(0);

			// if the currentNode == targetNode then finish
			if (currentNode == targetNode)
			{
				return currentNode;
			}

			// add current node to the closed list
			closedNodeHashSet.Add(currentNode);

			// evaluate fCost for each neighbour of current node
			EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
		}

		return null;
	}

	private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
	{
		Stack<Vector3> movementPathStack = new Stack<Vector3>();

		Node nextNode = targetNode;

		// Get mid point of cell
		Vector3 cellMidPoint = room.instanciatedRoom.grid.cellSize * 0.5f;

		cellMidPoint.z = 0f;

		while (nextNode != null)
		{
			// Convert grid position to world position
			Vector3 worldPosition = room.instanciatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x, nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

			worldPosition += cellMidPoint;

			movementPathStack.Push(worldPosition);

			nextNode = nextNode.parentNode;
		}

		return movementPathStack;
	}

	private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
	{
		Vector2Int currentNodeGridPosition = currentNode.gridPosition;

		Node validNeighbourNode;

		// Loop through all directions
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i == 0 && j == 0)
					continue;

				validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);

				if (validNeighbourNode != null)
				{
					// Calculate new gCost for neighbour
					int newCostToNeighbour;

					int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

					newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

					bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

					if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
					{
						validNeighbourNode.gCost = newCostToNeighbour;

						validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

						validNeighbourNode.parentNode = currentNode;

						if (!isValidNeighbourNodeInOpenList)
						{
							openNodeList.Add(validNeighbourNode);
						}
					}
				}
			}
		}
	}

	private static int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);

		int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}

		return 14 * dstX + 10 * (dstY - dstX);
	}

	private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
	{
		if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || 
			neighbourNodeXPosition < 0 || neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
		{
			return null;
		}

		// Get neighbour node
		Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

		int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

		// Check for moveable object at that position
		int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];

		if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
		{
			return null;
		}
		else
		{
			return neighbourNode;
		}
	}
}

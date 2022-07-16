using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Adjust positions by lower room bounds
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // Create open and closed lists
        var openNodeList = new List<Node>();
        var closedNodeList = new HashSet<Node>();

        // Create grid for pathfinding
        var gridNodes = new GridNodes(
            room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        var startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        var targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        var endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeList, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        var movementPathStack = new Stack<Vector3>();

        var nextNode = targetNode;

        // Get the midpoint of cell
        var cellMidpoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidpoint.z = 0f;

        while (nextNode != null)
        {
            // Convert grid position to world position
            var worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(
                nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y,
                0));

            worldPosition += cellMidpoint;

            movementPathStack.Push(worldPosition);

            // Continue up the chain in the path
            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes,
        List<Node> openNodeList, HashSet<Node> closedNodeList, InstantiatedRoom instantiatedRoom)
    {
        // Add start node to open list
        openNodeList.Add(startNode);

        // Loop through open list until empty
        while (openNodeList.Count > 0)
        {
            // Sort list by F cost
            openNodeList.Sort();

            // Current node = the node in the open list with the lowest F cost
            var currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // Finish if current node is the target
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // Add current node to closed list
            closedNodeList.Add(currentNode);

            // Evaluate current node's neighbours
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeList, instantiatedRoom);
        }

        return null;
    }

    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes,
        List<Node> openNodeList, HashSet<Node> closedNodeList, InstantiatedRoom instantiatedRoom)
    {
        var currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Don't evaluate current node
                if (i == 0 && j == 0) continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                    gridNodes, closedNodeList, instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    // Calculate new G cost for neighbour
                    var newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);

                    // Check if neighbour is in the open list
                    var isValidNeighbourInOpenList = openNodeList.Contains(validNeighbourNode);

                    // If new cost is less or neighbour is not in the open list update costs
                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        // Add neighbour to open list if it's not already there
                        if (!isValidNeighbourInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes,
        HashSet<Node> closedNodeList, InstantiatedRoom instantiatedRoom)
    {
        // Check if node position is within the grid
        var roomSize = GetRoomSize(instantiatedRoom);
        if (neighbourNodeXPosition >= roomSize.x || neighbourNodeXPosition < 0
            || neighbourNodeYPosition >= roomSize.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        var neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // Node is not valid if it is in the closed list
        if (closedNodeList.Contains(neighbourNode))
        {
            return null;
        }

        return neighbourNode;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        var dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        var dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        // The shortest of X and Y will be diagonal, with a cost of 14
        // and then subtract that distance from the longer, which will
        // be in a straight line, with a cost of 10
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY); // 10 used instead of 1, and 14 is pythagoras approximation
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private static Vector2Int GetRoomSize(InstantiatedRoom instantiatedRoom)
    {
        return new Vector2Int(
            instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x,
            instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y
        );
    }
}

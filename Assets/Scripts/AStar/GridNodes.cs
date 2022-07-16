using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNodes;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNodes[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int x, int y)
    {
        if (x < width && y < height)
        {
            return gridNodes[x, y];
        }
        else
        {
            Debug.Log($"Position ({x},{y}) is out of range of the grid (size {width},{height}))");
            return null;
        }
    }
}

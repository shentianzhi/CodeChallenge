using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public class Pathfinding : MonoBehaviour
{
    private int straightCost = 10;
    private int diagonalCost = 14;
    public GridSystem grid { private get; set; }

    //startPos start position; endPos, end position
    public List<int> FindPath(int2 startPos, int2 endPos, int2 gridSize)
    {
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                PathNode aNode = new PathNode();
                aNode.x = x;
                aNode.y = y;
                int gridIndex = aNode.index = GetIndex(x, y, gridSize.x);

                aNode.sCost = int.MaxValue;
                aNode.hCost = GetDistanceCost(new int2(x, y), endPos);
                aNode.SetFCost();

                aNode.isWalkable = grid.IsTileWalkable(gridIndex);
                aNode.lastIndex = -1;

                pathNodeArray[aNode.index] = aNode;
            }
        }

        NativeArray<int2> neibourOffsetArray = new NativeArray<int2>(new int2[]
        {
            // 8 direction
            new int2(-1,0),
            new int2(+1,0),
            new int2(0,+1),
            new int2(0,-1),
            new int2 (-1,-1),
            new int2 (-1,+1),
            new int2 (+1,-1),
            new int2 (+1,+1),
        }, Allocator.Temp);

        int endIndex = GetIndex(endPos.x, endPos.y, gridSize.x);

        PathNode startNode = pathNodeArray[GetIndex(startPos.x, startPos.y, gridSize.x)];
        startNode.sCost = 0;
        startNode.SetFCost();
        pathNodeArray[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);

        while (openList.Length > 0)
        {
            int currentIndex = GetLowestCostNodexIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentIndex];

            //When reach the last node
            if(currentIndex == endIndex)
            {
                break;
            }

            for (int i = 0; i < openList.Length; i++)
            {
                if(openList[i] == currentIndex)
                {
                    //RemoveAtSwapBack - Removes the element at the specified index and swaps the last element into its place.
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedList.Add(currentIndex);

            for (int i = 0; i < neibourOffsetArray.Length; i++)
            {
                int2 neighbourOffset = neibourOffsetArray[i];
                int2 neighbourPos = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                if (!IsPosInsideGrid(neighbourPos, gridSize))
                {
                    continue;
                }

                int neighbourIndex = GetIndex(neighbourPos.x, neighbourPos.y, gridSize.x);

                if (closedList.Contains(neighbourIndex))
                {
                    continue;
                }

                PathNode neighbourNode = pathNodeArray[neighbourIndex];
                if (!neighbourNode.isWalkable)
                {
                    continue;
                }

                int2 currentPos = new int2(currentNode.x, currentNode.y);
                
                //Update neighbouring nodes based on current node
                //Test the new sCost (sCostTemp). If sCostTemp < current neighbour sCost, then update the neighbour node.
                int sCostTemp = currentNode.sCost + GetDistanceCost(currentPos, neighbourPos);
                if (sCostTemp < neighbourNode.sCost)
                {
                    neighbourNode.lastIndex = currentNode.index;
                    neighbourNode.sCost = sCostTemp;
                    neighbourNode.SetFCost();
                    pathNodeArray[neighbourIndex] = neighbourNode;

                    if (!openList.Contains(neighbourNode.index))
                    {
                        openList.Add(neighbourNode.index);
                    }
                }
            }
        }

        List<int> path = new List<int>();
        //check if we find a path by checking the end node
        PathNode endNode = pathNodeArray[endIndex];
        //Not found
        if(endNode.lastIndex == -1)
        {
            Debug.Log("No path");
        }
        //Found a path
        else
        {
            path = CalculatePath(pathNodeArray, endNode);       
        }

        pathNodeArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
        neibourOffsetArray.Dispose();

        return path;
    }

    private List<int> CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode)
    {
        List<int> path = new List<int>();
        path.Add(endNode.index);

        PathNode currentNode = endNode;
        while (currentNode.lastIndex != -1)
        {
            PathNode lastNode = pathNodes[currentNode.lastIndex];
            path.Add(lastNode.index);
            currentNode = lastNode;
        }

        return path;
    }

    private bool IsPosInsideGrid(int2 gridPos, int2 gridSize)
    {
        return gridPos.x >= 0 && gridPos.x < gridSize.x
            && gridPos.y >= 0 && gridPos.y < gridSize.y;
    }

    /*
    789
    456
    123
    In for loop,
                           x   y   gridwidth
    for 3x3 grid, node 5 = 2 + 1 * 3 
     */
    private int GetIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    private int GetDistanceCost(int2 aPos, int2 bPos)
    {
        int xDis = math.abs(aPos.x - bPos.x);
        int yDis = math.abs(aPos.y - bPos.y);
        int remaining = math.abs(xDis - yDis);
        return diagonalCost * math.min(xDis, yDis) + straightCost * remaining;
    }

    private int GetLowestCostNodexIndex(NativeList<int> openList, NativeArray<PathNode> pathNodes)
    {
        PathNode lowestCostNode = pathNodes[openList[0]];
        for(int i = 1; i < openList.Length; i++)
        {
            PathNode aNode = pathNodes[openList[i]];
            if (aNode.fCost < lowestCostNode.fCost)
            {
                lowestCostNode = aNode;
            }
        }
        return lowestCostNode.index;
    }

    private struct PathNode
    {
        public int x;
        public int y;
        public int index;

        //cost moving from the start node
        public int sCost;
        //heuristic cost to reach end node.
        public int hCost;
        //final cost of a node, fCost = sCost + hCost. Compare fCost to find the path with lowest cost
        public int fCost;

        public bool isWalkable;

        public int lastIndex;

        public void SetFCost()
        {
            fCost = sCost + hCost;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
}

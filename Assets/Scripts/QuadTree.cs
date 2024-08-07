using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class QuadTree
{
    private int maxObjects = 10;
    private int maxLevels = 5;

    private int level;
    private List<Transform> objects;
    private Rect bounds;
    private QuadTree[] nodes;

    public QuadTree(int level, Rect bounds)
    {
        this.level = level;
        this.bounds = bounds;
        objects = new List<Transform>();
        nodes = new QuadTree[4];
    }

    public void Clear()
    {
        objects.Clear();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }
    }

    public void Split()
    {
        float subWidth = bounds.width / 2;
        float subHeight = bounds.height / 2;
        float x = bounds.x;
        float y = bounds.y;

        nodes[0] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
        nodes[1] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight));
        nodes[2] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
        nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    private int GetIndex(Rect pRect)
    {
        int index = -1;
        double verticalMidpoint = bounds.x + (bounds.width / 2);
        double horizontalMidpoint = bounds.y + (bounds.height / 2);

        bool topQuadrant = (pRect.y < horizontalMidpoint && pRect.y + pRect.height < horizontalMidpoint);
        bool bottomQuadrant = (pRect.y > horizontalMidpoint);

        if (pRect.x < verticalMidpoint && pRect.x + pRect.width < verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 1;
            }
            else if (bottomQuadrant)
            {
                index = 2;
            }
        }
        else if (pRect.x > verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 0;
            }
            else if (bottomQuadrant)
            {
                index = 3;
            }
        }

        return index;
    }

    public async Task InsertAsync(Transform obj)
    {
        if (nodes[0] != null)
        {
            int index = GetIndex(new Rect(obj.position.x, obj.position.y, 0, 0));

            if (index != -1)
            {
                await nodes[index].InsertAsync(obj);
                return;
            }
        }

        objects.Add(obj);

        if (objects.Count > maxObjects && level < maxLevels)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(new Rect(objects[i].position.x, objects[i].position.y, 0, 0));
                if (index != -1)
                {
                    nodes[index].Insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public void Insert(Transform obj)
    {
        if (nodes[0] != null)
        {
            int index = GetIndex(new Rect(obj.position.x, obj.position.y, 0, 0));

            if (index != -1)
            {
                nodes[index].Insert(obj);
                return;
            }
        }

        objects.Add(obj);

        if (objects.Count > maxObjects && level < maxLevels)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(new Rect(objects[i].position.x, objects[i].position.y, 0, 0));
                if (index != -1)
                {
                    nodes[index].Insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public List<Transform>  Retrieve(List<Transform> returnObjects, Rect pRect)
    {
        int index = GetIndex(pRect);
        if (index != -1 && nodes[0] != null)
        {
            nodes[index].Retrieve(returnObjects, pRect);
        }

        returnObjects.AddRange(objects);

        return returnObjects;
    }

    public void DrawDebug()
    {
        DrawNode(this);
    }

    private void DrawNode(QuadTree node)
    {
        if (node == null) return;

        Color color = Color.Lerp(Color.green, Color.red, node.level / maxLevels);
        Debug.DrawLine(new Vector3(node.bounds.x, node.bounds.y), new Vector3(node.bounds.x + node.bounds.width, node.bounds.y));
        Debug.DrawLine(new Vector3(node.bounds.x, node.bounds.y), new Vector3(node.bounds.x, node.bounds.y + node.bounds.height));
        Debug.DrawLine(new Vector3(node.bounds.x + node.bounds.width, node.bounds.y), new Vector3(node.bounds.x + node.bounds.width, node.bounds.y + node.bounds.height));
        Debug.DrawLine(new Vector3(node.bounds.x, node.bounds.y + node.bounds.height), new Vector3(node.bounds.x + node.bounds.width, node.bounds.y + node.bounds.height));

        for (int i = 0; i < node.nodes.Length; i++)
        {
            DrawNode(node.nodes[i]);
        }
    }
}

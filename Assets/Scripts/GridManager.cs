using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    public class TileInfo
    {
        public bool traversable;
        public bool visible;

        public TileInfo(bool traversable, bool visible)
        {
            this.traversable = traversable;
            this.visible = visible;
        }
    }

    public Tilemap traversable;
    public Tilemap notTraversable;
    public Dictionary<Vector2Int, TileInfo> map;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            traversable.CompressBounds();
            notTraversable.CompressBounds();
            map = new Dictionary<Vector2Int, TileInfo>();
            CreateGrid();
        }
    }

    public void CreateGrid()
    {
        for (int x = traversable.cellBounds.xMin; x < traversable.cellBounds.xMax; x++)
        {
            for (int y = traversable.cellBounds.yMin; y < traversable.cellBounds.yMax; y++)
            {
                Vector3 worldPosition = traversable.CellToWorld(new Vector3Int(x, y, 0));
                if (notTraversable.HasTile(notTraversable.WorldToCell(worldPosition)))
                {
                    map.Add(new Vector2Int(x, y), new TileInfo(false, false));
                }
                else
                {
                    map.Add(new Vector2Int(x, y), new TileInfo(true, true));
                }
            }
        }

    }

    public void SetTraversability(Vector2Int gridPos, bool traversability)
    {
        if (map.ContainsKey(gridPos))
        {
            map[gridPos].traversable = traversability; 
        }
    }

    public Vector2 GetTileCenter(Vector2Int gridPos)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(gridPos, out tile);
        Vector3Int posn = new Vector3Int(gridPos.x, gridPos.y, 0);

        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }

        if (tile.traversable)
        {
            return (Vector2)traversable.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        }
        else
        {
            return (Vector2)notTraversable.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        }
    }

    private void TintTile(Vector2Int gridPos, Color color)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(gridPos, out tile);
        Vector3Int posn = new Vector3Int(gridPos.x, gridPos.y, 0);

        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }

        if (tile.traversable)
        {
            traversable.SetTileFlags(posn, TileFlags.None);
            traversable.SetColor(posn, color);
        }
        else
        {
            traversable.SetTileFlags(posn, TileFlags.None);
            notTraversable.SetColor(posn, color);
        }

    }

    public void TintTiles(List<Vector2Int> tiles, Color color)
    {
        foreach (var tile in map)
        {
            if (tiles.Contains(tile.Key))
            {
                TintTile(tile.Key, color);
            }
            else
            {
                TintTile(tile.Key, Color.white);
            }
        }
    }

    public Vector2Int GetCellPosition(Vector3 worldPos)
    {
        Vector3Int pos3 = traversable.WorldToCell(worldPos);
        Vector2Int pos = new Vector2Int(pos3.x, pos3.y);
        return pos;
    }

    public Vector2Int MouseToGrid()
    {
        return GetCellPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (map.ContainsKey(new Vector2Int(position.x - 1, position.y)))
        {
            neighbors.Add(new Vector2Int(position.x - 1, position.y));
        }

        if (map.ContainsKey(new Vector2Int(position.x + 1, position.y)))
        {
            neighbors.Add(new Vector2Int(position.x + 1, position.y));
        }

        if (map.ContainsKey(new Vector2Int(position.x, position.y - 1)))
        {
            neighbors.Add(new Vector2Int(position.x, position.y - 1));
        }

        if (map.ContainsKey(new Vector2Int(position.x, position.y + 1)))
        {
            neighbors.Add(new Vector2Int(position.x, position.y + 1));
        }

        return neighbors;

    }
    public class NodeInfo
    {
        // which position on the map does this correspond to
        public Vector2Int position;

        // parent node
        public NodeInfo parent;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (!(obj is NodeInfo))
                return false;

            NodeInfo info = (NodeInfo)obj;
            // compare elements here
            return info.position == this.position;
        }

        public override int GetHashCode()
        {
            return (int)position.GetHashCode();
        }

        public List<NodeInfo> NeighborsToNodeInfos(List<Vector2Int> neighbors)
        {
            List<NodeInfo> nodeInfos = new List<NodeInfo>();

            foreach (Vector2Int neighbor in neighbors)
            {
                NodeInfo current = new NodeInfo();
                current.position = neighbor;
                current.parent = this;
                nodeInfos.Add(current);
            }

            return nodeInfos;
        }

    }


    public List<NodeInfo> IndicateTraversible(Vector2Int startingSquare, int range)
    {
        PriorityQueue<NodeInfo, int> toSearch = new PriorityQueue<NodeInfo, int>();
        NodeInfo start = new NodeInfo();
        start.position = startingSquare;
        start.parent = null;
        toSearch.Enqueue(start, 0);

        List<NodeInfo> searched = new List<NodeInfo>();

        while (toSearch.Count > 0)
        {
            int currentDist;
            NodeInfo current;
            toSearch.TryPeek(out current, out currentDist);
            toSearch.Dequeue();
            searched.Add(current);

            List<NodeInfo> neighbors = current.NeighborsToNodeInfos(GetNeighbors(current.position));

            foreach (NodeInfo neighbor in neighbors)
            {
                // if were out of our range, ignore these nodes
                int distance = currentDist + 1;
                if (distance > range)
                {
                    break;
                }

                // if it isnt traversible, ignore this node
                if (!map[neighbor.position].traversable)
                {
                    continue;
                }

                // we like this node, make it
                NodeInfo toAdd = new NodeInfo();
                toAdd.position = neighbor.position;
                toAdd.parent = current;

                // if already in searched list, dont add
                if (searched.Contains(toAdd))
                {
                    continue;
                }

                bool inSearch = toSearch.UnorderedItems.Select(a => a.Element).Contains(toAdd);

                if (!inSearch)
                {
                    toSearch.Enqueue(toAdd, distance);
                }

            }

        }

        return searched;

    }

    public List<Vector2Int> FindClosestTraversible(Vector2Int startingSquare, int numSquaresToFind)
    {
        PriorityQueue<NodeInfo, int> toSearch = new PriorityQueue<NodeInfo, int>();
        NodeInfo start = new NodeInfo();
        start.position = startingSquare;
        start.parent = null;
        toSearch.Enqueue(start, 0);

        List<NodeInfo> searched = new List<NodeInfo>();

        while (toSearch.Count > 0)
        {
            int currentDist;
            NodeInfo current;
            toSearch.TryPeek(out current, out currentDist);
            toSearch.Dequeue();
            searched.Add(current);

            List<NodeInfo> neighbors = current.NeighborsToNodeInfos(GetNeighbors(current.position));

            foreach (NodeInfo neighbor in neighbors)
            {
                // store dist
                int distance = currentDist + 1;

                // if weve already found x squares, were good
                if (searched.Count > numSquaresToFind)
                {
                    break;
                }

                // if it isnt traversible, ignore this node
                if (!map[neighbor.position].traversable)
                {
                    continue;
                }

                // we like this node, make it
                NodeInfo toAdd = new NodeInfo();
                toAdd.position = neighbor.position;
                toAdd.parent = current;

                // if already in searched list, dont add
                if (searched.Contains(toAdd))
                {
                    continue;
                }

                bool inSearch = toSearch.UnorderedItems.Select(a => a.Element).Contains(toAdd);

                if (!inSearch)
                {
                    toSearch.Enqueue(toAdd, distance);
                }

            }

        }

        return searched.Where(a => a.position != startingSquare).Select(a => a.position).ToList();


    }


}

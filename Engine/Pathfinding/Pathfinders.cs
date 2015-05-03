using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// -------------------------------------------------------------
// PATHFINDING

// A * 

// from xna
public class Point: System.Object {
    public int x, y;
    
    public Point(int xValue, int yValue) {
        x = xValue;
        y = yValue;
    }
    
    public override bool Equals(System.Object obj) {
        // Check for null values and compare run-time types.
        if (obj == null || GetType() != obj.GetType()) 
            return false;
        
        Point p = (Point)obj;
        return (x == p.x) && (y == p.y);
    }
    
    public override int GetHashCode() {
        return x ^ y;
    }
    
    public override string ToString() {
        return string.Format("Point: ({0},{1})", x, y);
    }
}

public class Cell : IComparable {
    
    public bool reachable = false;
    public int x = 0;
    public int y = 0;
    public Cell parent = null;
    public double g = 0;
    public double h = 0;
    public double f = 0;
    public List<Cell> path = new List<Cell>();
    
    public Cell(int x, int y, bool reachable) {
        
        this.reachable = reachable;
        this.x = x;
        this.y = y;
        this.parent = null;
        this.g = 0;
        this.h = 0;
        this.f = 0;
    }
    
    // Compare 2 cells F values
    // 
    // returns -1, 0 or 1 if lower, equal or greater
    
    int IComparable.CompareTo(object obj) {
        Cell cell = (Cell)obj;
        if (this.f > cell.f)
            return 1;
        if (this.f < cell.f)
            return -1;
        else
            return 0;
        
    }
    
    private class sortCell: IComparer {
        int IComparer.Compare(object a, object b) {
            Cell c1 = (Cell)a;
            Cell c2 = (Cell)b;
            if (c1.f > c2.f)
                return 1;
            if (c1.f < c2.f)
                return -1;
            else
                return 0;
        }
    }
    
    public static IComparer sortCellDescending() {      
        return (IComparer)new sortCell();
    }
}

public class AStar {    
    public HeapQ<Cell> heapQ = null;
    public HashSet<Cell> closed = null;
    public List<Cell> cells = null;
    public int gridHeight = 6;
    public int gridWidth = 6;
    public List<Point> tilesBlocked;
    public bool debugMode = false;
    
    public AStar() {
        Init();
    }
    
    public void Init() {
        heapQ = new HeapQ<Cell>();
        
        tilesBlocked = new List<Point>();
        
        closed = new HashSet<Cell>();
        cells = new List<Cell>();
        
        gridHeight = 6;
        gridWidth = 6;
    }
    
    public void DisplayGraph(List<Point> path = null) {
        
        if (!debugMode) {
            return;
        }
        
        Debug.Log("DISPLAYING GRAPH");
        
        if (path != null) {
            Debug.Log("w/ PATH");
        }
        
        Debug.Log(GetGraphOutput(path));
    }
    
    public string GetGraphOutput(List<Point> path = null) {
        
        // Output graph for debuggin 
        
        System.Text.StringBuilder gridFull = new System.Text.StringBuilder();
        
        for (int y = 0; y < gridHeight; y++) {
            
            System.Text.StringBuilder gridRow = new System.Text.StringBuilder();
            
            for (int x = 0; x < gridWidth; x++) {
                
                if (IsNonWalkable(x, y)) {
                    gridRow.Append("#");
                }
                else {
                    
                    string itemCode = " ";
                    
                    if (path != null) {
                        // check if in path
                        if (IsInPath(x, y, path)) {
                            itemCode = "X";
                        }
                    }
                    
                    gridRow.Append(itemCode);
                }
            }
            
            gridFull.Append(gridRow.ToString());
            gridFull.Append("\r\n"); // linebreak
        }
        
        return gridFull.ToString();
    }
    
    public bool IsInPath(int x, int y, List<Point> path) {
        foreach (Point p in path) {
            if (p.x == x && p.y == y) {
                return true;
            }
        }
        return false;
    }
    
    public bool IsNonWalkable(int x, int y) {
        foreach (Point point in tilesBlocked) {
            if (point.x == x && point.y == y) {
                return true;
            }
        }
        
        return false;
    }
    
    public void GenerateGraph(List<Point> nodes, int height, int width) {
        tilesBlocked = nodes;
        
        DisplayGraph();
        
        closed = new HashSet<Cell>();
        
        cells = new List<Cell>();
        
        gridHeight = height;
        gridWidth = width;
        
        bool reachable = false;
        
        for (int x = 0; x < gridWidth; x++) {
            
            for (int y = 0; y < gridHeight; y++) {
                
                Point node = new Point(x, y);
                
                if (nodes.Contains(node)) {             
                    reachable = false;
                }
                else {
                    reachable = true;
                }
                cells.Add(new Cell(x, y, reachable));
            }
        }
    }
    
    public List<Point> GetPath(Point start, Point end) {
        
        Cell cellStart = GetCell(start.x, start.y);
        Cell cellEnd = GetCell(end.x, end.y);
        
        //
        
        Process(cellStart, cellEnd);
        
        List<Point> path = new List<Point>();
        
        Cell cell = cellEnd;
        
        if (cell != null) {
            path.Add(new Point(cell.x, cell.y));
            
            if (cell.parent != null) {
                while (cell.parent != cellStart && cell.parent != null) {
                    cell = cell.parent;
                    path.Add(new Point(cell.x, cell.y));
                }
            }
        }
        
        DisplayGraph(path);
        
        return path;
    }
    
    // Compute the heuristic value H for a cell: distance between
    // this cell and the ending cell multiply by 10.
    // returns heuristic value H
    
    public double GetHeuristic(Cell cell, Cell start, Cell end) {
        return 10 * (Math.Abs(cell.x - end.x) + Math.Abs(cell.y - end.y));
    }
    
    // Returns a cell from the cells list
    //  x cell x coordinate
    //  y cell y coordinate
    // returns cell
    public Cell GetCell(int x, int y) { 
        //int i = x * gridHeight + y;
        //if(cells.Count > i) {
        return cells[x * gridHeight + y];       
        //}
        //else {
        //  return null;
        //}
    } 
    
    // Get adjacent cells
    
    // Returns adjacent cells to a cell. Clockwise starting
    // from the one on the right.
    // 
    // cell get adjacent cells for this cell
    // returns adjacent cells list 
    
    public List<Cell> GetAdjacentCells(Cell cell) {
        
        List<Cell> cells = new List<Cell>();
        
        if (cell.x < gridWidth - 1) {
            
            Cell cellItem = GetCell(cell.x + 1, cell.y);
            if (cellItem != null) {
                cells.Add(cellItem);
            }
        }
        
        if (cell.y > 0) {
            
            Cell cellItem = GetCell(cell.x, cell.y - 1);
            if (cellItem != null) {
                cells.Add(cellItem);
            }
        }
        
        if (cell.x > 0) {
            
            Cell cellItem = GetCell(cell.x - 1, cell.y);            
            if (cellItem != null) {
                cells.Add(cellItem);
            }
        }
        
        if (cell.y < gridHeight - 1) {
            
            Cell cellItem = GetCell(cell.x, cell.y + 1);            
            if (cellItem != null) {
                cells.Add(cellItem);
            }
        }
        
        return cells;
    }
    
    // UPDATE CELL
    // Update adjacent cell
    // 
    // adj adjacent cell to current cell
    // cell current cell being processed
    
    public Cell UpdateCell(Cell adjCell, Cell cell, Cell start, Cell end) {
        adjCell.g = cell.g + 10;
        adjCell.h = GetHeuristic(adjCell, start, end);
        adjCell.parent = cell;
        adjCell.f = adjCell.h + adjCell.g;
        return adjCell;
    }
    
    public void Process(Cell start, Cell end) {
        
        // add starting cell to open heap queue
        heapQ.Push(start);
        
        while (heapQ.items.Count > 0) {
            // pop the cell from the heap queue
            Cell cell = heapQ.Pop();
            
            // add cell to the closes list so we don't process it twice
            closed.Add(cell);
            
            // if ending cell, display found path
            if (cell == end) {
                // process path(start,end)
                break;
            }
            
            // get adjacent cells for cell
            List<Cell> adjCells = GetAdjacentCells(cell);
            
            foreach (Cell adjCell in adjCells) {
                
                if (adjCell.reachable && !closed.Contains(adjCell)) {
                    
                    if (heapQ.items.Contains(adjCell)) {
                        
                        // if adj cell in open list, check if current path is
                        // better than the one previously found
                        // for this adj cell.
                        
                        if (adjCell.g > cell.g + 10) {
                            UpdateCell(adjCell, cell, start, end);
                        }
                    }
                    else {
                        UpdateCell(adjCell, cell, start, end);
                        
                        // add adj cell to open list
                        
                        heapQ.Push(adjCell);                                
                    }
                }           
            }
        }
    }

    // TESTS    
    
    public string[,] TestLevel1() {
        
        string[,] rows = new string[9, 9] 
        { 
            { "#", "#", "#", "#", "#", "#", "#", "#", "#"}, 
            { "#", " ", " ", "2", " ", " ", " ", " ", "#"},
            { "#", " ", " ", " ", " ", " ", " ", " ", "#"},
            { "#", "#", "#", "#", "#", "#", " ", "#", "#"},
            { "#", " ", " ", " ", " ", " ", " ", " ", "#"},
            { "#", " ", "1", " ", " ", " ", " ", " ", "#"},
            { "#", " ", " ", " ", " ", " ", " ", " ", "#"},
            { "#", " ", " ", " ", " ", " ", " ", " ", "#"},
            { "#", "#", "#", "#", "#", "#", "#", "#", "#"}
        };
        
        return rows;
    }
    
    public void TestAStarDefault() {
        
        Debug.Log("Running TestAStarDefault"); 
        
        AStar astar = new AStar();
        astar.Init();
        
        List<Point> nodes = new List<Point>();
        
        string[,] map = TestLevel1();
        
        int currentHeight = 0;
        int currentWidth = 0;
        
        if (map != null) {
            currentHeight = map.GetLength(0);
            currentWidth = map.GetLength(1);
        }
        
        Point point1 = new Point(1, 4);
        Point point2 = new Point(1, 1);
        
        for (int y = 0; y < currentHeight; y += 1) {
            for (int x = 0; x < currentWidth; x += 1) {
                if (map[y, x] == "#") {
                    nodes.Add(new Point(x, y));
                }
                else if (map[y, x] == "1") {
                    point1 = new Point(x, y);
                }
                else if (map[y, x] == "1") {
                    point2 = new Point(x, y);
                }
            }
        }
        
        Debug.Log("point1");
        Debug.Log(point1.ToJson());
        
        Debug.Log("point2");
        Debug.Log(point2.ToJson());
        
        astar.GenerateGraph(nodes, currentHeight, currentWidth);
        
        Debug.Log("nodes");
        Debug.Log(nodes.ToJson());
        
        List<Point> path = astar.GetPath(point1, point2); 
        
        Debug.Log("path");
        Debug.Log(path.ToJson());
        
        if (path != null && path.Count > 0) {
            Debug.Log("TESTS SUCCESSFUL");
        }
        else {
            Debug.LogError("TESTS IN ERROR");
        }
        
    }
}

// -------------------------------------------------------------
// A minimal re-implementation of the Python heapq module.
// http://entitycrisis.blogspot.com/2011/08/heap-queue-in-c.html

public class HeapQ<T> where T : IComparable {
    public List<T> items;
    
    public HeapQ() {
        items = new List<T>();
    }
    
    public bool Empty {
        get { return items.Count == 0; }
    }
    
    public T First {
        get {
            if (items.Count > 1) {
                return items[0];
            }
            return items[items.Count - 1];
        }
    }
    
    public void Push(T item) {
        items.Add(item);
        SiftDown(0, items.Count - 1);
    }
    
    public T Pop() {
        T item;
        var last = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        if (items.Count > 0) {
            item = items[0];
            items[0] = last;
            SiftUp(0);
        }
        else {
            item = last;
        }
        return item;
    }
    
    void SiftDown(int startpos, int pos) {
        var newitem = items[pos];
        while (pos > startpos) {
            var parentpos = (pos - 1) >> 1;
            var parent = items[parentpos];
            if (parent.CompareTo(newitem) <= 0)
                break;
            items[pos] = parent;
            pos = parentpos;
        }
        items[pos] = newitem;
    }
    
    void SiftUp(int pos) {
        var endpos = items.Count;
        var startpos = pos;
        var newitem = items[pos];
        var childpos = 2 * pos + 1;
        while (childpos < endpos) {
            var rightpos = childpos + 1;
            if (rightpos < endpos && items[rightpos].CompareTo(items[childpos]) <= 0)
                childpos = rightpos;
            items[pos] = items[childpos];
            pos = childpos;
            childpos = 2 * pos + 1;
        }
        items[pos] = newitem;
        SiftDown(startpos, pos);
    }
}

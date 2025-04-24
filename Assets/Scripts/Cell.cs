// Assets/Scripts/Cell.cs
public class Cell
{
    public bool Visited;
    public bool WallTop, WallBottom, WallLeft, WallRight;

    public Cell()
    {
        Visited = false;
        WallTop = WallBottom = WallLeft = WallRight = true;
    }
}
// This class represents a cell in the maze. It has properties to indicate whether it has been visited and whether it has walls on each side.
// The constructor initializes the cell as unvisited and with walls on all sides.
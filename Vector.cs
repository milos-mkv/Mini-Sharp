namespace MiniSharp;

public class Vector
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vector(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public void Print()
    {
        Console.WriteLine($"Vector X: {this.X}, Y: {this.Y}");
    }
}
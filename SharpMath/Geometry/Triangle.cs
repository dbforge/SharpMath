namespace SharpMath.Geometry
{
    public class Triangle : Polygon
    {
        public Triangle()
        {
        }

        public Triangle(Point2D p1, Point2D p2, Point2D p3) : base(p1, p2, p3)
        { }

        public Vector2 Base { get; }
        public double Height { get; }
    }
}

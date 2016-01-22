namespace SharpMath.Geometry
{
    public class Point3D : Vector3
    {
        public Point3D(double x, double y, double z) : base(x, y, z)
        { }

        public Vector3 ToVector()
        {
            return FromVector(this);
        }
    }
}

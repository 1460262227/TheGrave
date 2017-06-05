using System;

namespace Nova
{
    public struct Pos
    {
        public int x;
        public int y;

        public Pos(int px, int py)
        {
            x = px;
            y = py;
        }

        public static Pos operator +(Pos a, Pos b)
        {
            return new Pos(a.x + b.x, a.y + b.y);
        }

        public static Pos operator -(Pos a, Pos b)
        {
            return new Pos(a.x - b.x, a.y - b.y);
        }

        public static Pos operator *(Pos a, int s)
        {
            return new Pos(a.x * s, a.y * s);
        }

        public static bool operator ==(Pos a, Pos b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Pos a, Pos b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public override bool Equals(object obj)
        {
            return this == (Pos)obj;
        }

        public override int GetHashCode()
        {
            return x + y;
        }

        public float Dist(Pos to)
        {
            var dx = x - to.x;
            var dy = y - to.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
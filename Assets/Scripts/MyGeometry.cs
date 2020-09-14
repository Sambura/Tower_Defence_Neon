using UnityEngine;

namespace MyGeometry
{
	/* 
	public struct Vector2
	{
		public float x, y;

		public static Vector2 positiveInfinity { get; } = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		public static Vector2 negativeInfinity { get; } = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}
	*/
	public struct Line
	{
		public float A, B, C; // Coefficients in line equation: Ax + By + C = 0

		public Line(Vector2 a, Vector2 b)
		{
			A = a.y - b.y;
			B = b.x - a.x;
			C = a.x * b.y - b.x * a.y;
		}
	}

	public struct Segment
	{
		public Line line; // Line containing this segment
		public Vector2 a, b; // Segment vertices
		public float minX, minY, maxX, maxY; // Basically the rectangle containing this segment

		// Basically checks, if point belongs to this segment (given point should belong to this.line)
		public bool IsBelongToRect(Vector2 c)
		{
			return (minX <= c.x && c.x <= maxX
				&&
				minY <= c.y && c.y <= maxY);
		}

		public Segment(Vector2 a, Vector2 b)
		{
			line = new Line(a, b);
			this.a = a; this.b = b;
			minX = Mathf.Min(a.x, b.x);
			minY = Mathf.Min(a.y, b.y);
			maxX = Mathf.Max(a.x, b.x);
			maxY = Mathf.Max(a.y, b.y);
		}
	}

	public static class Geometry2D
	{
		static public Vector2 LineIntersection(Line a, Line b)
		{
			float denominator = a.A * b.B - b.A * a.B;
			if (Mathf.Approximately(denominator, 0)) // Mathf.Approximately checks if two float numbers are similar
			{
				if (Mathf.Approximately(a.C, b.C)) return Vector2.positiveInfinity; // Lines are congruent

				return Vector2.negativeInfinity; // Lines are parallel, no intersection
			}

			return new Vector2(
				(a.B * b.C - b.B * a.C) / denominator,
				(b.A * a.C - a.A * b.C) / denominator
				); // Lines intersection point
		}

		static public Vector2 SegmentIntersection(Segment a, Segment b)
		{
			var c = LineIntersection(a.line, b.line);
			if (c == Vector2.negativeInfinity) return c; // Segments are parallel but don't lie on the same line, no intersection

			if (c == Vector2.positiveInfinity)
				if (a.IsBelongToRect(b.a) || a.IsBelongToRect(b.b))
					return c; // Segments lie on the same line and have common points - infinite intersections
				else
					return Vector2.negativeInfinity; // Segments lie on the same line but do not have common points - no intersection

			if (a.IsBelongToRect(c) && b.IsBelongToRect(c))
				return c; // Segments do intersect under nonzero angle
			else
				return Vector2.negativeInfinity; // Lines intersect, but intersection point doesn't belong to the both segments
		}
	}
}
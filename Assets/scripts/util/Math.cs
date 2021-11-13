using UEMath = UnityEngine.Mathf;

public class Math {

	/**
	 * Rotate a vector (x, y) in the standard basis (i.e., {(1,0), (0,1)})
	 * ang degrees.
	 *
	 * To rotate a force from global space to camera space, just use the
	 * camera's transform.eulerAngles.y as the angle.
	 *
	 * @param x: The vector's horizontal component.
	 * @param y: The vector's vertical component.
	 * @param ang: The rotation angle, in degrees.
	 * @return The resulting 2D Vector.
	 */
	public static (float, float) RotateVec2(float x, float y, float ang) {
		/**
		 *
		 * The explanation for why this works is lengthy... I wish you luck!
		 *
		 * This rotation takes a vector (x, y) on the orthonormal, standard
		 * basis {u, v} (which is simply a fancy way to say the usual basis)
		 * and rotates it to the new basis {m, n}, where both m and n are t
		 * degrees apart from u and v, respectively. I.e.:
		 *
		 *                n (a, b) _   ^   _ m (c, d)
		 * ^ v (0, 1)             |\   |v  /|
		 * |                        \t°|  /
		 * |                =====>   \ | / t°
		 * |-----> u (1, 0)           \|/-------> u
		 *
		 * The first thing that must be figured out is how the vector m
		 * relates to u, and how n relates to v. Hopefully the diagram
		 * bellow should make some sense as to why the horizontal component
		 * of m must be cos(t) and the vertical component must be sin(t).
		 *
		 * ^ v
		 * | cos(t)
		 * |----
		 * |  _ m|
		 * |  /| | sin(t)
		 * | /   |
		 * |/-----------> u
		 *
		 * n is defined in a similar way, however it's always on the
		 * quadrant adjacent to m and u, and thus is 90 degrees apart. So,
		 * although n may be represented as {cos(t+90), sin(t+90)}, from
		 * the trigonometric identities we can calculate it as
		 * {-sin(t), cos(t)}.
		 *
		 *                 ^ v
		 *       cos(t+90) |
		 *            -----|
		 *           |n _  |
		 * sin(t+90) | |\  |
		 *           |   \ |
		 *     -----------\|---------> u
		 *
		 * In short, the rotation takes a vector on the basis {(0,1), (1,0)}
		 * and rotates it to the basis {(cos(t), sin(t)), (-sin(t), cos(t)}.
		 *
		 * From Analytical Geometry, we know that the following matrix may
		 * be used to calculate the change of basis of the vector (x,y) in
		 * the standard basis to the vector (rx, ry) in the basis {m, n}
		 * (just bear with me... I got this matrix from a book, and thus it
		 * has to be correct™):
		 *
		 * | rx |   |  cos(t)  sin(t) |   | x |
		 * |    | = |                 | * |   |
		 * | ry |   | -sin(t)  cos(t) |   | y |
		 *
		 * So, the rotated vector (rx, ry) can be calculated as:
		 *
		 * rx =  x * cos(t) + y * sin(t)
		 * ry = -x * sin(t) + y * cos(t)
		 *
		 * And indeed, if the angle is 0 (and thus it's not rotated at all)
		 * the matrix becomes:
		 *
		 * |  cos(0)  sin(0) |   | 1   0 |
		 * |                 | = |       |
		 * | -sin(0)  cos(0) |   | 0   1 |
		 *
		 * And rotating it to the right, we would get:
		 *
		 * |  cos(90)  sin(90) |   |  0   1 |
		 * |                   | = |        |
		 * | -sin(90)  cos(90) |   | -1   0 |
		 *
		 * So, using this for movement, facing right and moving forward in
		 * the local coordinates will result in a horizontal movement on the
		 * global coordinates.
		 */
		float t = UEMath.Deg2Rad * ang;
		float cos = UEMath.Cos(t);
		float sin = UEMath.Sin(t);

		float rx =  x * cos + y * sin;
		float ry = -x * sin + y * cos;

		return (rx, ry);
	}
}

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

	/**
	 * Rotate a vector (x, y, z) in the standard basis (i.e., {(1,0,0),
	 * (0,1,0), (0,0,1)}) horAng degrees on the horizontal plane and verAng
	 * degrees on the vertical plane.
	 *
	 * @param x: The vector's horizontal component.
	 * @param y: The vector's vertical component.
	 * @param z: The vector's depth component.
	 * @param horAng: The rotation angle on the horizontal plane, in degrees.
	 * @param verAng: The rotation angle on the vertcal plane, in degrees.
	 * @return The resulting 3D Vector.
	 */
	public static (float, float, float) RotateVec3(float x, float y,
			float z, float horAng, float verAng) {
		/**
		 * The explanation for this is similar to RotateVec2's explanation.
		 * So, familiarize yourself with that before proceeding!
		 *
		 * The rotation is done in two step: first in the horizontal plane
		 * (i.e., the XZ plane around the Y axis), and then around the
		 * vertical plane (i.e., ZY plane around the local X axis). The XZ
		 * plane is rotate "theta" (t) degrees, and the ZY plane is rotate
		 * "phi" (p) degrees.
		 *
		 * The matrices were devised in a similar way:
		 *
		 *      |  cos(t)  sin(t) |       |  cos(p)  sin(p) |
		 * XZ = |                 |  ZY = |                 |
		 *      | -sin(t)  cos(t) |       | -sin(p)  cos(p) |
		 *
		 * However, to merge the matrices, the axis must be in the same
		 * order. Thus, the ZY matrix must actually be:
		 *
		 *      | cos(p) -sin(p) |
		 * ZY = |                |
		 *      | sin(p)  cos(p) |
		 *
		 * Before applying those transformations to a 3D vector, these
		 * matrices must be expanded to 3x3 matrices, keeping the vector in
		 * the axis of rotation unchanged:
		 *
		 *      |  cos(t)  0  sin(t) |       |  1    0       0    |
		 *      |                    |       |                    |
		 * XZ = |    0     1    0    |  ZY = |  0  cos(p)  sin(p) |
		 *      |                    |       |                    |
		 *      | -sin(t)  0  cos(t) |       |  0 -sin(p)  cos(p) |
		 *
		 * Then, merging those two transformations into a single operation
		 * is as simple as multiplying the two matrices, which gives the
		 * unified transformation as:
		 *
		 * | rx |   |  cos(t)   sin(t) * sin(p)   sin(t) * cos(p) |   | x |
		 * |    |   |                                             |   |   |
		 * | ry | = |   0            cos(p)           -sin(p)     | * | y |
		 * |    |   |                                             |   |   |
		 * | rz |   | -sin(t)   cos(t) * sin(p)   cos(t) * cos(p) |   | z |
		 */
		float t = horAng * UEMath.Deg2Rad;
		float ct = UEMath.Cos(t);
		float st = UEMath.Sin(t);

		float p = verAng * UEMath.Deg2Rad;
		float cp = UEMath.Cos(p);
		float sp = UEMath.Sin(p);

		float rx =  x * ct + y * st * sp + z * st * cp;
		float ry =  x * 0  + y * cp      - z * sp;
		float rz = -x * st + y * ct * sp + z * ct * cp;

		return (rx, ry, rz);
	}

	/**
	 * Normalize an angle to the [0, 360] range, assuming that the angle
	 * haven't done multiple revolutions. If it has done multiple
	 * revolutions, then this will simply take multiple calls to fully
	 * normalize the angle.
	 *
	 * @param angle: The angle, in degrees, to be normalized.
	 * @return The normalized angle, still in degrees.
	 */
	public static float NormalizeAngle(float angle) {
		if (angle > 360.0f) {
			angle -= 360.0f;
		}
		else if (angle < 0.0f) {
			angle += 360.0f;
		}

		return angle;
	}

	/**
	 * Calculate the difference between two angles. The result is always in
	 * the range [0, 180].
	 *
	 * @param a: The first angle. Must be between [0, 360]!
	 * @param b: The second angle. Must be between [0, 360]!
	 */
	public static float DiffAngle(float a, float b) {
		float ret;

		/* Convert a and b to [-180, 180] */
		if (a > 180.0f) {
			a = -180.0f + (a - 180.0f);
		}
		if (b > 180.0f) {
			b = -180.0f + (b - 180.0f);
		}

		ret = UEMath.Abs(a - b);
		if (ret > 180.0f) {
			return 360.0f - ret;
		}
		return ret;
	}
}

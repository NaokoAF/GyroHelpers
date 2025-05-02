using System.Numerics;
using System.Runtime.CompilerServices;

namespace GyroHelpers.GyroSpaces;

/// <summary>
/// Local Space implementation. Maps 2 of the 3 gyro axes to the output.
/// <para>Simplest implementation. Mainly useful for handheld consoles and phones.</para>
/// </summary>
public class LocalGyroSpace : IGyroSpace
{
	/// <summary>
	/// Gyro axis used for output's X axis. This is equivalent to vertical movement.
	/// </summary>
	public GyroAxis AxisX { get; set; } = GyroAxis.Pitch;

	/// <summary>
	/// Gyro axis used for output's Y axis. This is equivalent to horizontal movement.
	/// </summary>
	public GyroAxis AxisY { get; set; } = GyroAxis.Yaw;

	public LocalGyroSpace(GyroAxis axisX, GyroAxis axisY)
	{
		AxisX = axisX;
		AxisY = axisY;
	}

	public LocalGyroSpace()
	{
	}

	public Vector2 Transform(GyroState gyro)
	{
		return new Vector2(GetAxis(gyro.Gyro, AxisX), GetAxis(gyro.Gyro, AxisY));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float GetAxis(Vector3 vector, GyroAxis axis)
	{
#if NET7_0_OR_GREATER
		return vector[(int)axis];
#else
		switch (axis)
		{
			case GyroAxis.Pitch: return vector.X;
			case GyroAxis.Yaw: return vector.Y;
			case GyroAxis.Roll: return vector.Z;
			default: throw new ArgumentOutOfRangeException(nameof(axis));
		}
#endif
	}
}

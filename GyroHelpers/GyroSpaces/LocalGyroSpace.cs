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
	/// Gyro axis used for output's yaw axis (Y).
	/// </summary>
	public GyroAxis YawAxis { get; set; } = GyroAxis.Yaw;

	public LocalGyroSpace(GyroAxis yawAxis)
	{
		YawAxis = yawAxis;
	}

	public LocalGyroSpace()
	{
	}

	public Vector2 Transform(GyroState gyro)
	{
		// invert roll axis
		gyro.Gyro.Z = -gyro.Gyro.Z;

		return new Vector2(gyro.Gyro.X, GetAxis(gyro.Gyro, YawAxis));
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

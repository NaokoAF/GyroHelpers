using System.Numerics;

namespace GyroHelpers.GyroSpaces;

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

	public Vector2 Transform(Gyroscope gyro)
	{
		return new Vector2(gyro.Gyro[(int)AxisX], gyro.Gyro[(int)AxisY]);
	}
}

using System.Numerics;

namespace GyroHelpers;

public record struct Gyroscope
{
	/// <summary>
	/// Calibrated gyroscope value in radians per second.
	/// </summary>
	public Vector3 Gyro;

	/// <summary>
	/// Accelerometer value in meters per second squared.
	/// </summary>
	public Vector3 Accelerometer;

	/// <summary>
	/// Normalized gravity vector.
	/// </summary>
	public Vector3 Gravity;

	public Gyroscope(Vector3 gyro, Vector3 accelerometer, Vector3 gravity)
	{
		Gyro = gyro;
		Accelerometer = accelerometer;
		Gravity = gravity;
	}
}

using System.Numerics;

namespace GyroHelpers;

public record struct Gyroscope
{
	/// <summary>
	/// Calibrated gyroscope value.
	/// </summary>
	public Vector3 Gyro;

	/// <summary>
	/// Accelerometer value.
	/// </summary>
	public Vector3 Accelerometer;

	/// <summary>
	/// Normalized gravity vector.
	/// </summary>
	public Vector3 Gravity;

	/// <summary>
	/// Sensor timestamp in nanoseconds.
	/// </summary>
	public ulong Timestamp;

	public Gyroscope(Vector3 gyro, Vector3 accelerometer, Vector3 gravity, ulong timestamp)
	{
		Gyro = gyro;
		Accelerometer = accelerometer;
		Gravity = gravity;
		Timestamp = timestamp;
	}
}

using System.Numerics;

namespace GyroHelpers.Gravity;

/// <summary>
/// An algorithm for calculating gravity from an accelerometer and gyroscope.
/// </summary>
public interface IGravityCalculator
{
	Vector3 Update(Vector3 gyro, Vector3 accelerometer, float deltaTime);
	void Reset();
}
using System.Numerics;

namespace GyroHelpers.Gravity;

public interface IGravityCalculator
{
	Vector3 Update(Vector3 gyro, Vector3 accelerometer, float deltaTime);
	void Reset();
}
using System.Numerics;

namespace GyroHelpers.GyroSpaces;

/// <summary>
/// An algorithm for converting gyro inputs into a 2-axis output.
/// </summary>
public interface IGyroSpace
{
	Vector2 Transform(Gyroscope gyro);
}

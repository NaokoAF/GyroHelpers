using System.Numerics;

namespace GyroHelpers.GyroSpaces;

public interface IGyroSpace
{
	Vector2 Transform(Gyroscope gyro);
}

using System.Numerics;
using System.Runtime.CompilerServices;

namespace GyroHelpers;

/// <summary>
/// Averages multiple gyro samples together.
/// </summary>
public class GyroMovingAverage
{
	Vector3 value;
	int sampleCount;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 Add(Vector3 gyro)
	{
		sampleCount++;

		if (sampleCount == 1)
			value = gyro;
		else
			value += (gyro - value) / sampleCount;

		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset()
	{
		value = Vector3.Zero;
		sampleCount = 0;
	}
}

using System.Numerics;

namespace GyroHelpers;

public class GyroAcceleration
{
	/// <summary>
	/// Threshold at which to use <see cref="SensitivitySlow"/>. Value in radians.
	/// </summary>
	public float ThresholdSlow { get; set; }

	/// <summary>
	/// Threshold at which to use <see cref="SensitivityFast"/>. Value in radians.
	/// </summary>
	public float ThresholdFast { get; set; }

	/// <summary>
	/// Sensitivity to use at <see cref="ThresholdSlow"/>.
	/// </summary>
	public float SensitivitySlow { get; set; }

	/// <summary>
	/// Sensitivity to use at <see cref="ThresholdFast"/>.
	/// </summary>
	public float SensitivityFast { get; set; }

	public GyroAcceleration(float thresholdSlow, float thresholdFast, float sensitivitySlow, float sensitivityFast)
	{
		ThresholdSlow = thresholdSlow;
		ThresholdFast = thresholdFast;
		SensitivitySlow = sensitivitySlow;
		SensitivityFast = sensitivityFast;
	}

	public Vector2 Transform(Vector2 gyro)
	{
		if (ThresholdFast <= ThresholdSlow)
			return gyro * SensitivitySlow;

		// map gyro speed from (ThresholdSlow -> ThresholdFast) to (SensitivitySlow -> SensitivityFast)
		float sensitivity = MathHelper.Remap(gyro.Length(), ThresholdSlow, ThresholdFast, SensitivitySlow, SensitivityFast);
		return gyro * sensitivity;
	}
}

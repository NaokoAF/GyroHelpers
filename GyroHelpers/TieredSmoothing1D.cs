namespace GyroHelpers;

public class TieredSmoothing1D
{
	/// <summary>
	/// Inputs below this threshold will be smoothed.
	/// <para>If changing this value, also change <see cref="ThresholdDirect"/>.</para>
	/// </summary>
	public float ThresholdSmooth { get; set; }

	/// <summary>
	/// Values above this threshold won't be smoothed.
	/// <para>If changing this value, also change <see cref="ThresholdSmooth"/>.</para>
	/// </summary>
	public float ThresholdDirect { get; set; }

	/// <summary>
	/// Amount of time to smooth for.
	/// <para>See <see cref="ThresholdSmooth"/> and <see cref="ThresholdDirect"/>.</para>
	/// </summary>
	public float SmoothTime { get => movingAverage.TimeWindow; set => movingAverage.TimeWindow = value; }

	TimedMovingAverage movingAverage;

	public TieredSmoothing1D(float smoothTime, float thresholdSmooth, float thresholdDirect, int inputsPerSecond)
	{
		ThresholdSmooth = thresholdSmooth;
		ThresholdDirect = thresholdDirect;
		movingAverage = new(smoothTime, inputsPerSecond);
	}

	public float Apply(float input, float deltaTime)
	{
		if (SmoothTime <= 0 || ThresholdDirect <= ThresholdSmooth)
			return input;

		// maps input magnitude to a value between 0 and 1, where 0 is thresholdSmooth and 1 is thresholdDirect
		// we then interpolate between direct and smoothed inputs based on this value
		// this means inputs below thresholdSmooth get smoothed, inputs above thresholdDirect dont, and everything in between gets a mix of both
		float inputMagnitude = Math.Abs(input);
		float weight = MathHelper.Clamp(MathHelper.InverseLerp(ThresholdSmooth, ThresholdDirect, inputMagnitude), 0f, 1f);
		float directInput = input * weight;
		float smootherInput = movingAverage.Add(deltaTime, input * (1f - weight));
		return directInput + smootherInput;
	}

	public void Reset()
	{
		movingAverage.Reset();
	}
}

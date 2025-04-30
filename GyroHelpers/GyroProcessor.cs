using GyroHelpers.GyroSpaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace GyroHelpers;

public class GyroProcessor
{
	/// <summary>
	/// Algorithm used for converting 3-axis gyro into a 2-axis output.
	/// </summary>
	public IGyroSpace GyroSpace { get; set; }

	/// <summary>
	/// Gyro acceleration.
	/// </summary>
	public GyroAcceleration Acceleration { get; } = new(0f, 0f, 1f, 1f);

	/// <summary>
	/// Rotations below this amount will get squished to 0.
	/// <para>This can help increase precision for small movements, while also reducing the effects of shaky hands, without a deadzone.</para>
	/// </summary>
	public float TighteningThreshold { get; set; }

	/// <summary>
	/// Rotations below this threshold will be smoothed. Value in radians per second.
	/// <para>If changing this value, also change <see cref="SmoothingThresholdDirect"/>.</para>
	/// </summary>
	public float SmoothingThresholdSmooth { get => smoothing.ThresholdSmooth; set => smoothing.ThresholdSmooth = value; }

	/// <summary>
	/// Rotations above this threshold won't be smoothed. Value in radians per second.
	/// <para>A good default is double the value of <see cref="SmoothingThresholdSmooth"/>.</para>
	/// </summary>
	public float SmoothingThresholdDirect { get => smoothing.ThresholdDirect; set => smoothing.ThresholdDirect = value; }

	/// <summary>
	/// Amount of time to smooth rotations by.
	/// <para>See <see cref="SmoothingThresholdSmooth"/> and <see cref="SmoothingThresholdDirect"/>.</para>
	/// </summary>
	public float SmoothingTime { get => smoothing.SmoothTime; set => smoothing.SmoothTime = value; }

	/// <summary>
	/// When <see cref="MomentumActive"/> is true, reduces gyro input until it stops. Radians per second.
	/// </summary>
	public Vector2 MomentumFriction { get => momentum.Friction; set => momentum.Friction = value; }

	/// <summary>
	/// Pauses gyro input and keeps momentum. See <see cref="MomentumFriction"/>
	/// </summary>
	public bool MomentumActive { get => momentum.Active; set => momentum.Active = value; }

	TieredSmoothing2D smoothing = new(0.1f, 0f, 0f, 256);
	GyroMomentum momentum = new();

	public GyroProcessor(IGyroSpace gyroSpace)
	{
		GyroSpace = gyroSpace;
	}

	public GyroProcessor() : this(new PlayerTurnGyroSpace())
	{
	}

	/// <summary>
	/// Feed gyro through processing chain and convert it into an angle change.
	/// </summary>
	/// <param name="gyro">Gyro data, usually coming from <see cref="GyroInput"/>.</param>
	/// <param name="deltaTime">Time since last call in seconds.</param>
	/// <returns>Processed gyro delta in radians.</returns>
	public Vector2 Update(Gyroscope gyro, float deltaTime)
	{
		Vector2 result = GyroSpace.Transform(gyro);
		result = smoothing.Apply(result, deltaTime);
		result = ApplyTightening(result, TighteningThreshold);
		result = Acceleration.Transform(result);
		result = momentum.Update(result, deltaTime);
		return result * deltaTime;
	}

	/// <summary>
	/// Reset internal state.
	/// </summary>
	public void Reset()
	{
		smoothing.Reset();
		momentum.Reset();
	}

	// squeezes everything below threshold down to 0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 ApplyTightening(Vector2 input, float threshold)
	{
		float magnitude = input.Length();
		if (magnitude < threshold)
		{
			return input * (magnitude / threshold);
		}
		else
		{
			return input;
		}
	}
}

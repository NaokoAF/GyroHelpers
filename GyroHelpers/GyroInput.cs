using GyroHelpers.Gravity;
using System.Numerics;

namespace GyroHelpers;

/// <summary>
/// Handles gyro calibration and gravity calculation.
/// </summary>
public class GyroInput
{
	/// <summary>
	/// Current gyro state.
	/// </summary>
	public Gyroscope Gyro => new(gyro, accel, gravity, prevTimestamp ?? 0);

	/// <summary>
	/// Calibrate gyro bias to correct drift.
	/// </summary>
	public bool Calibrating
	{
		get => calibrating;
		set
		{
			if (calibrating != value)
				biasCalculator.Reset();

			calibrating = value;
		}
	}

	/// <summary>
	/// Calibrated gyro bias.
	/// </summary>
	public Vector3 Bias => bias;

	bool calibrating;
	Vector3 gyro;
	Vector3 accel;
	Vector3 gravity;
	Vector3 bias;
	ulong? prevTimestamp;
	IGravityCalculator gravityCalculator;
	GyroMovingAverage biasCalculator = new();
	GyroMovingAverage averageCalculator = new();

	/// <summary>
	/// Creates a GyroInput.
	/// </summary>
	/// <param name="gravityCalculator">Algorithm for calculating gravity.</param>
	public GyroInput(IGravityCalculator gravityCalculator)
	{
		this.gravityCalculator = gravityCalculator;
	}

	public GyroInput() : this(new JibbGravityCalculator())
	{
	}

	/// <summary>
	/// Begin input frame. Call this before polling for gyro input.
	/// </summary>
	public void Begin()
	{
		averageCalculator.Reset();
	}

	/// <summary>
	/// Feed gyro into calibration, and gravity calculator. Call this for every received gyro sample.
	/// </summary>
	/// <param name="gyro">Gyroscope in radians per second</param>
	/// <param name="timestamp">Timestamp in nanoseconds</param>
	public void InputGyro(Vector3 gyro, ulong timestamp)
	{
		if (Calibrating)
		{
			bias = biasCalculator.Add(gyro);
			ClearState();
			return;
		}

		// unbias
		gyro -= bias;

		// calculate an average from samples received since Begin()
		this.gyro = averageCalculator.Add(gyro);

		// calculate gravity
		float deltaTime = (timestamp - (prevTimestamp ?? timestamp)) / 1000000000f;
		gravity = gravityCalculator.Update(gyro, accel, deltaTime);
		gravity = gravity != Vector3.Zero ? Vector3.Normalize(gravity) : Vector3.Zero; // normalize
		prevTimestamp = timestamp;
	}

	/// <summary>
	/// Feed accelerometer into gravity calculator. Call this for every received accelerometer sample.
	/// </summary>
	/// <param name="accel">Accelerometer in meters per second squared</param>
	/// <param name="timestamp">Timestamp in nanoseconds</param>
	public void InputAccelerometer(Vector3 accel, ulong timestamp)
	{
		this.accel = accel;
	}

	/// <summary>
	/// Reset internal state and calibration.
	/// </summary>
	public void Reset()
	{
		ClearState();
		bias = Vector3.Zero;
		biasCalculator.Reset();
	}

	void ClearState()
	{
		gyro = Vector3.Zero;
		accel = Vector3.Zero;
		gravity = Vector3.Zero;
		prevTimestamp = null;
		averageCalculator.Reset();
		gravityCalculator.Reset();
	}
}
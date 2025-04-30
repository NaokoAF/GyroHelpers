using System.Numerics;

namespace GyroHelpers;

/// <summary>
/// Allows gyro input to be paused, while keeping it's momentum.
/// </summary>
public class GyroMomentum
{
	/// <summary>
	/// Pauses gyro input and keeps momentum. See <see cref="Friction"/>
	/// </summary>
	public bool Active { get; set; }

	/// <summary>
	/// When <see cref="Active"/> is true, reduces gyro input until it stops. Radians per second.
	/// </summary>
	public Vector2 Friction { get; set; }

	Vector2 momentum;

	public GyroMomentum(Vector2 friction)
	{
		Friction = friction;
	}

	public GyroMomentum()
	{
	}

	public Vector2 Update(Vector2 gyro, float deltaTime)
	{
		if (Active)
		{
			Vector2 newMomentum = Vector2.Abs(momentum) - Friction * deltaTime;
			newMomentum = Vector2.Max(newMomentum, Vector2.Zero);
			momentum = Vector2.CopySign(newMomentum, momentum);
			return momentum;
		}
		else
		{
			momentum = gyro;
			return gyro;
		}
	}

	public void Reset()
	{
		momentum = Vector2.Zero;
	}
}

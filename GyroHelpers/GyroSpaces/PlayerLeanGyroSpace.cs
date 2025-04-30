using System.Numerics;

namespace GyroHelpers.GyroSpaces;

/// <summary>
/// Player Space (Lean) implementation from <see href="http://gyrowiki.jibbsmart.com/blog:player-space-gyro-and-alternatives-explained">GyroWiki</see>.
/// Takes gravity into account for a more natural translation of movements.
/// <para>Tilting the controller like a wheel, relative to the player, counts as horizontal movement.</para>
/// </summary>
public class PlayerLeanGyroSpace : IGyroSpace
{
	public float RollRelaxFactor { get; set; } = 1.15f;

	public Vector2 Transform(Gyroscope gyro)
	{
		// project pitch axis onto gravity plane
		Vector3 pitchVector = Vector3.UnitX - gyro.Gravity * gyro.Gravity.X;
		Vector3 rollVector = Vector3.Cross(pitchVector, gyro.Gravity);

		// normalize. it'll be zero if pitch and gravity are parallel, which we ignore
		if (rollVector != Vector3.Zero)
		{
			rollVector = Vector3.Normalize(rollVector);

			// some info about the controller's orientation that we'll use to smooth over boundaries
			float flatness = Math.Abs(gyro.Gravity.Y); // 1 when controller is flat
			float upness = Math.Abs(gyro.Gravity.Z); // 1 when controller is upright
			float sideReduction = MathHelper.Clamp((Math.Max(flatness, upness) - 0.125f) / 0.125f, 0f, 1f);

			float worldRoll = gyro.Gyro.Y * rollVector.Y + gyro.Gyro.Z * rollVector.Z; // dot product but just yaw and roll
			float gyroMagnitude = MathHelper.Sqrt(gyro.Gyro.Y * gyro.Gyro.Y + gyro.Gyro.Z * gyro.Gyro.Z); // magnitude but just yaw and roll
			float yaw = -Math.Sign(worldRoll) * sideReduction * Math.Min(Math.Abs(worldRoll) * RollRelaxFactor, gyroMagnitude);
			return new Vector2(gyro.Gyro.X, yaw);
		}

		return new Vector2(gyro.Gyro.X, 0f);
	}
}

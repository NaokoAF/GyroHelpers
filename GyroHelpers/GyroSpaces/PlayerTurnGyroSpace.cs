using System.Numerics;

namespace GyroHelpers.GyroSpaces;

/// <summary>
/// Player Space (Turn) implementation from <see href="http://gyrowiki.jibbsmart.com/blog:player-space-gyro-and-alternatives-explained">GyroWiki</see>.
/// Takes gravity into account for a more natural translation of movements.
/// <para>Turning the controller side to side, relative to the player, counts as horizontal movement.</para>
/// </summary>
public class PlayerTurnGyroSpace : IGyroSpace
{
	public float YawRelaxFactor { get; set; } = 2f;

	public Vector2 Transform(GyroState gyro)
	{
		// use world yaw for yaw direction, local combined yaw for magnitude
		float worldYaw = gyro.Gyro.Y * gyro.Gravity.Y + gyro.Gyro.Z * gyro.Gravity.Z; // dot product but just yaw and roll
		float gyroMagnitude = MathHelper.Sqrt(gyro.Gyro.Y * gyro.Gyro.Y + gyro.Gyro.Z * gyro.Gyro.Z); // magnitude but just yaw and roll

		float yaw = -Math.Sign(worldYaw) * Math.Min(Math.Abs(worldYaw) * YawRelaxFactor, gyroMagnitude);
		return new Vector2(gyro.Gyro.X, yaw);
	}
}

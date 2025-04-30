using System.Numerics;
using System.Runtime.CompilerServices;

namespace GyroHelpers;

/// <summary>
/// Flick Stick implementation. Based on <see href="http://gyrowiki.wikidot.com/blog:good-gyro-controls-part-2:the-flick-stick">Jibb Smart's implementation.</see>
/// </summary>
public class FlickStick
{
	/// <summary>
	/// Stick threshold to active a flick.
	/// </summary>
	public float FlickThreshold { get; set; } = 0.9f;

	/// <summary>
	/// How long a flick takes in seconds.
	/// </summary>
	public float FlickTime { get; set; } = 0.1f;

	/// <summary>
	/// Snap flicks to cardinal directions. Affected by <see cref="SnappingStrength"/>.
	/// </summary>
	public FlickSnapping Snapping { get; set; }

	/// <summary>
	/// Size of forward snap. Only applies when <see cref="Snapping"/> is <see cref="FlickSnapping.ForwardOnly"/>. Value in radians.
	/// </summary>
	public float SnappingForwardDeadzone { get; set; } = 7f * MathHelper.DegreesToRadians;

	/// <summary>
	/// Flick snapping strength between 0 and 1.
	/// </summary>
	public float SnappingStrength { get; set; } = 1f;

	/// <summary>
	/// Rotations below this threshold will be smoothed. This is to account for the controller's stick resolution. Value in radians.
	/// <para>If changing this value, also change <see cref="SmoothingThresholdDirect"/>.</para>
	/// </summary>
	public float SmoothingThresholdSmooth { get => smoothing.ThresholdSmooth; set => smoothing.ThresholdSmooth = value; }

	/// <summary>
	/// Rotations above this threshold won't be smoothed. This is to account for the controller's stick resolution. Value in radians.
	/// <para>A good default is double the value of <see cref="SmoothingThresholdSmooth"/>.</para>
	/// </summary>
	public float SmoothingThresholdDirect { get => smoothing.ThresholdDirect; set => smoothing.ThresholdDirect = value; }

	/// <summary>
	/// Amount of time to smooth rotations for.
	/// <para>See <see cref="SmoothingThresholdSmooth"/> and <see cref="SmoothingThresholdDirect"/>.</para>
	/// </summary>
	public float SmoothingTime { get => smoothing.SmoothTime; set => smoothing.SmoothTime = value; }

	/// <summary>
	/// Whether a flick is currently active.
	/// </summary>
	public bool IsFlicking => flicking;

	bool flicking;
	float flickProgress = 1f;
	float flickAngle;
	float? lastStickAngle;
	TieredSmoothing1D smoothing = new(0.064f, 0.02f, 0.04f, 256); // JSM default settings

	/// <summary>
	/// Process stick input and convert it into an angle change.
	/// </summary>
	/// <param name="stick">Controller stick vector.</param>
	/// <param name="deltaTime">Time since last call in seconds.</param>
	/// <returns>Angle delta in radians.</returns>
	public float Update(Vector2 stick, float deltaTime)
	{
		float result = 0f;
		if (stick.LengthSquared() >= FlickThreshold * FlickThreshold)
		{
			float stickAngle = MathHelper.Atan2(-stick.X, -stick.Y);
			if (!flicking)
			{
				// initiate flick at this angle
				flicking = true;
				flickAngle = ApplySnapping(stickAngle, Snapping, SnappingStrength, SnappingForwardDeadzone);
				flickProgress = 0;

				// no flick animation. increment instantly
				if (FlickTime <= 0)
					result += flickAngle;
			}
			else
			{
				// add any angle changes after the initial flick
				float deltaAngle = WrapDeltaAngle(stickAngle - (lastStickAngle ?? stickAngle));
				result += smoothing.Apply(deltaAngle, deltaTime);
			}
			lastStickAngle = stickAngle;
		}
		else if (flicking)
		{
			flicking = false;
			lastStickAngle = null;
			smoothing.Reset();
		}

		// update flick animation
		if (flickProgress < 1f && FlickTime > 0)
		{
			// move progress forward
			float lastFlickProgress = flickProgress;
			flickProgress = Math.Min(flickProgress + (deltaTime / FlickTime), 1f);

			// apply easing (optional but looks weird without it)
			float lastT = EaseOutCubic(lastFlickProgress);
			float t = EaseOutCubic(flickProgress);
			result += (t - lastT) * flickAngle;
		}
		return result;
	}

	/// <summary>
	/// Reset internal state.
	/// </summary>
	public void Reset()
	{
		flicking = false;
		flickProgress = 1f;
		flickAngle = 0f;
		lastStickAngle = null;
		smoothing.Reset();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float ApplySnapping(float angle, FlickSnapping snap, float snapStrength, float forwardDeadzone)
	{
		float interval;
		switch (snap)
		{
			case FlickSnapping.Two:
				interval = MathHelper.PI;
				break;
			case FlickSnapping.Four:
				interval = MathHelper.PI * 0.5f;
				break;
			case FlickSnapping.Six:
				interval = MathHelper.PI * 0.333333f;
				break;
			case FlickSnapping.Eight:
				interval = MathHelper.PI * 0.25f;
				break;
			case FlickSnapping.ForwardOnly:
				// apply forward deadzone using snapStrength
				if (Math.Abs(angle) < forwardDeadzone)
					return MathHelper.Lerp(angle, 0f, snapStrength);
				else
					return angle;
			default:
				return angle;
		}

		float snappedAngle = MathHelper.Round(angle / interval) * interval;
		return MathHelper.Lerp(angle, snappedAngle, snapStrength);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float WrapDeltaAngle(float value)
	{
		value = (value + MathHelper.PI) % MathHelper.Tau;
		if (value < 0)
			value += MathHelper.Tau;
		return value - MathHelper.PI;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static float EaseOutCubic(float t)
	{
		t -= 1f;
		return 1 + t * t * t;
	}
}

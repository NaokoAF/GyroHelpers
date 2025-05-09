using System.Runtime.CompilerServices;

namespace GyroHelpers;

/// <summary>
/// Useful math functions used all around GyroHelpers.
/// Includes wrappers for functions only available on newer .NET versions.
/// </summary>
public static class MathHelper
{
	public const float PI = 3.14159265f;
	public const float Tau = 6.283185307f;
	public const float DegreesToRadians = PI / 180f;
	public const float RadiansToDegrees = 180f / PI;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Lerp(float from, float to, float t)
	{
		return from + (to - from) * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float InverseLerp(float from, float to, float value)
	{
		return (value - from) / (to - from);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap(float value, float fromA, float fromB, float toA, float toB)
	{
		return toA + (toB - toA) * Clamp((value - fromA) / (fromB - fromA), 0f, 1f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Exp2(float x)
	{
#if NETCOREAPP2_0_OR_GREATER
		return MathF.Pow(2, x);
#else
		return (float)Math.Pow(2, x);
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Round(float value)
	{
#if NETCOREAPP2_0_OR_GREATER
		return MathF.Round(value);
#else
		return (float)Math.Round(value);
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp(float value, float min, float max)
	{
#if NETCOREAPP2_0_OR_GREATER
		return Math.Clamp(value, min, max);
#else
		if (value < min) return min;
		else if (value > max) return max;
		else return value;
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Atan2(float y, float x)
	{
#if NETCOREAPP2_0_OR_GREATER
		return MathF.Atan2(y, x);
#else
		return (float)Math.Atan2(y, x);
#endif
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Sqrt(float x)
	{
#if NETCOREAPP2_0_OR_GREATER
		return MathF.Sqrt(x);
#else
		return (float)Math.Sqrt(x);
#endif
	}
}

namespace FoodDrinkApp.Services;

/// <summary>
/// Pure validation helpers for nutrition-related user input.
/// Separated from ViewModels so validation logic can be unit-tested
/// independently of MAUI platform dependencies.
/// </summary>
public static class NutritionValidator
{
    /// <summary>Minimum accepted daily calorie goal (kcal).</summary>
    public const int MinCalorieGoal = 500;

    /// <summary>Maximum accepted daily calorie goal (kcal).</summary>
    public const int MaxCalorieGoal = 10000;

    /// <summary>Default calorie goal when none is set.</summary>
    public const int DefaultCalorieGoal = 2000;

    /// <summary>
    /// Clamps a user-provided calorie goal to the valid range
    /// [<see cref="MinCalorieGoal"/>, <see cref="MaxCalorieGoal"/>].
    /// Values outside this range are silently clamped;
    /// the caller can detect clamping by comparing the return
    /// value to the input.
    /// </summary>
    /// <param name="value">Raw calorie goal value (may be negative or
    /// unreasonably large).</param>
    /// <returns>A calorie goal guaranteed to be within the accepted
    /// nutrition-tracking range.</returns>
    public static int ClampCalorieGoal(int value)
    {
        if (value < MinCalorieGoal)
            return MinCalorieGoal;
        if (value > MaxCalorieGoal)
            return MaxCalorieGoal;
        return value;
    }

    /// <summary>
    /// Determines whether the supplied value is within the valid
    /// calorie-goal range without clamping it.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <returns><c>true</c> when <paramref name="value"/> falls inside
    /// [<see cref="MinCalorieGoal"/>, <see cref="MaxCalorieGoal"/>];
    /// otherwise <c>false</c>.</returns>
    public static bool IsValidCalorieGoal(int value)
    {
        return value >= MinCalorieGoal && value <= MaxCalorieGoal;
    }
}

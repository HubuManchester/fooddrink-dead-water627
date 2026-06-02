using FoodDrinkApp.Services;
using Xunit;

namespace FoodDrinkApp.Tests;

/// <summary>
/// Unit tests for <see cref="NutritionValidator"/> —
/// the pure validation logic backing the SettingsViewModel
/// daily calorie goal clamping behaviour.
/// All tests follow the AAA (Arrange-Act-Assert) pattern.
/// </summary>
public class NutritionValidatorTests
{
    // ──────────────────────────────────────────────
    //  ClampCalorieGoal — happy-path and boundary
    // ──────────────────────────────────────────────

    /// <summary>
    /// A value within the valid range should be returned unchanged.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_ValidInput_ReturnsUnchanged()
    {
        // Arrange
        const int input = 2000;

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(2000, result);
    }

    /// <summary>
    /// The exact minimum boundary (500) should be accepted as-is.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_MinimumBoundary_ReturnsMinimum()
    {
        // Arrange
        const int input = NutritionValidator.MinCalorieGoal; // 500

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MinCalorieGoal, result);
    }

    /// <summary>
    /// The exact maximum boundary (10 000) should be accepted as-is.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_MaximumBoundary_ReturnsMaximum()
    {
        // Arrange
        const int input = NutritionValidator.MaxCalorieGoal; // 10000

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MaxCalorieGoal, result);
    }

    // ──────────────────────────────────────────────
    //  ClampCalorieGoal — below-range clamping
    // ──────────────────────────────────────────────

    /// <summary>
    /// A negative number should be clamped up to the minimum (500).
    /// This simulates a user accidentally typing a negative value.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_NegativeInput_ClampsToMinimum()
    {
        // Arrange
        const int input = -150;

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MinCalorieGoal, result);
    }

    /// <summary>
    /// Zero should be clamped up to the minimum (500).
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_ZeroInput_ClampsToMinimum()
    {
        // Arrange
        const int input = 0;

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MinCalorieGoal, result);
    }

    /// <summary>
    /// A value just below the minimum boundary should be clamped up.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_JustBelowMinimum_ClampsToMinimum()
    {
        // Arrange
        const int input = NutritionValidator.MinCalorieGoal - 1; // 499

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MinCalorieGoal, result);
    }

    // ──────────────────────────────────────────────
    //  ClampCalorieGoal — above-range clamping
    // ──────────────────────────────────────────────

    /// <summary>
    /// A value exceeding 10 000 should be clamped down to the maximum.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_AboveMaximum_ClampsToMaximum()
    {
        // Arrange
        const int input = 15000;

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MaxCalorieGoal, result);
    }

    /// <summary>
    /// A value just above the maximum boundary should be clamped down.
    /// </summary>
    [Fact]
    public void ClampCalorieGoal_JustAboveMaximum_ClampsToMaximum()
    {
        // Arrange
        const int input = NutritionValidator.MaxCalorieGoal + 1; // 10001

        // Act
        var result = NutritionValidator.ClampCalorieGoal(input);

        // Assert
        Assert.Equal(NutritionValidator.MaxCalorieGoal, result);
    }

    // ──────────────────────────────────────────────
    //  IsValidCalorieGoal
    // ──────────────────────────────────────────────

    /// <summary>
    /// The default goal (2000) should be considered valid.
    /// </summary>
    [Fact]
    public void IsValidCalorieGoal_Default_ReturnsTrue()
    {
        // Arrange
        const int input = NutritionValidator.DefaultCalorieGoal;

        // Act
        var result = NutritionValidator.IsValidCalorieGoal(input);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// A negative value should be considered invalid.
    /// </summary>
    [Fact]
    public void IsValidCalorieGoal_Negative_ReturnsFalse()
    {
        // Arrange
        const int input = -1;

        // Act
        var result = NutritionValidator.IsValidCalorieGoal(input);

        // Assert
        Assert.False(result);
    }
}

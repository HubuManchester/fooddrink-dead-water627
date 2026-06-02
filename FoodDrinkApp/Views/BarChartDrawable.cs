namespace FoodDrinkApp.Views;

/// <summary>
/// Draws a horizontal bar chart for category-level statistics (e.g. average
/// calories per star rating group) using <see cref="Microsoft.Maui.Graphics"/>.
/// Optionally renders a vertical goal reference line when
/// <see cref="GoalCalories"/> is set, linking directly to the daily calorie
/// goal preference from Settings.
/// </summary>
public sealed class BarChartDrawable : IDrawable
{
    /// <summary>
    /// List of (label, numeric value, fill colour) entries to render as bars.
    /// </summary>
    public List<(string Label, float Value, Color Color)> Bars { get; set; } = [];

    /// <summary>
    /// Daily calorie goal from user preferences.  When greater than zero
    /// a dashed vertical reference line is drawn so the user can visually
    /// compare per-rating averages against their target.
    /// </summary>
    public int GoalCalories { get; set; }

    /// <inheritdoc />
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;

        // ── Empty-state fallback ─────────────────────
        if (Bars.Count == 0)
        {
            canvas.Font = new Microsoft.Maui.Graphics.Font("OpenSansRegular");
            canvas.FontSize = 15;
            canvas.FontColor = Color.FromArgb("#9E9E9E");
            canvas.DrawString(
                "No data to display",
                dirtyRect.Width / 2f - 60, dirtyRect.Height / 2f - 10, 120, 20,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            return;
        }

        // ── Scale calculations ───────────────────────
        float maxValue = Math.Max(Bars.Max(b => b.Value), GoalCalories > 0 ? GoalCalories : 0);
        if (maxValue <= 0) maxValue = 1;

        float marginLeft   = 70f;
        float marginRight  = 8f;
        float marginTop    = 12f;
        float marginBottom = 24f;

        // Reserve a fixed zone on the right for numeric value labels
        // so they never get pushed off-screen even when bars are wide.
        const float labelZoneWidth = 55f;

        float chartLeft    = marginLeft;
        float chartRight   = dirtyRect.Width - marginRight;
        float chartWidth   = chartRight - chartLeft;

        // Bars must not exceed chartWidth minus the label zone,
        // guaranteeing every numeric label stays in-bounds.
        float barMaxWidth  = chartWidth - labelZoneWidth;
        if (barMaxWidth < 20f) barMaxWidth = 20f;

        float barAreaHeight = dirtyRect.Height - marginTop - marginBottom;

        float barHeight  = Math.Min(28f, barAreaHeight / Bars.Count - 8f);
        float barSpacing = (barAreaHeight - barHeight * Bars.Count) / (Bars.Count + 1);

        // ── Goal reference line ──────────────────────
        if (GoalCalories > 0)
        {
            // Scale the goal line position using the bar-visible width
            // so it stays correctly aligned with rendered bars.
            float goalX = chartLeft + GoalCalories / maxValue * barMaxWidth;
            goalX = Math.Clamp(goalX, chartLeft, chartLeft + barMaxWidth);

            canvas.StrokeColor = Color.FromArgb("#F29B38");
            canvas.StrokeSize  = 2f;
            canvas.StrokeDashPattern = [6f, 4f];
            canvas.DrawLine(goalX, marginTop, goalX, dirtyRect.Height - marginBottom);

            // Goal label sits in the reserved label zone, right-aligned.
            canvas.FontSize  = 9;
            canvas.FontColor = Color.FromArgb("#F29B38");
            canvas.DrawString(
                $"Goal: {GoalCalories}",
                chartLeft + barMaxWidth + 2, dirtyRect.Height - marginBottom + 2,
                labelZoneWidth - 2, 12,
                HorizontalAlignment.Right, VerticalAlignment.Top);

            canvas.StrokeDashPattern = null;
        }

        // ── Render each bar ──────────────────────────
        float y = marginTop + barSpacing;

        foreach (var bar in Bars)
        {
            // Scale to the bar-visible width so the longest bar
            // ends exactly where the label zone begins.
            float barWidth = bar.Value / maxValue * barMaxWidth;
            if (barWidth < 3) barWidth = 3;

            // Filled bar rectangle (4 px corner radius)
            canvas.FillColor = bar.Color;
            canvas.FillRoundedRectangle(chartLeft, y, barWidth, barHeight, 4);

            // Numeric value label — drawn inside the reserved zone,
            // right of the bar, fully protected from clipping.
            canvas.FontSize = 11;
            canvas.FontColor = Color.FromArgb("#555555");
            canvas.DrawString(
                $"{bar.Value:F0}",
                chartLeft + barMaxWidth + 2, y, labelZoneWidth - 2, barHeight,
                HorizontalAlignment.Left, VerticalAlignment.Center);

            // Category label left of the bar
            canvas.FontSize = 12;
            canvas.FontColor = Color.FromArgb("#333333");
            canvas.DrawString(
                bar.Label,
                4, y, marginLeft - 8, barHeight,
                HorizontalAlignment.Right, VerticalAlignment.Center);

            y += barHeight + barSpacing;
        }
    }
}

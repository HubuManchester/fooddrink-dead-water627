namespace FoodDrinkApp.Views;

/// <summary>
/// Draws a horizontal bar chart for category-level statistics (e.g. average
/// calories per star rating group) using <see cref="Microsoft.Maui.Graphics"/>.
/// Zero external dependencies — rendered directly by a MAUI <c>GraphicsView</c>.
/// </summary>
/// <remarks>
/// <para>Layout math:</para>
/// <list type="bullet">
///   <item>Left margin (70 px) reserved for category labels.</item>
///   <item>Remaining width scaled so the longest bar fills the chart area.</item>
///   <item>Bars are rounded rectangles with 4 px corner radius.</item>
///   <item>Vertical spacing distributed equally between bars.</item>
/// </list>
/// </remarks>
public sealed class BarChartDrawable : IDrawable
{
    /// <summary>
    /// List of (label, numeric value, fill colour) entries to render as bars.
    /// Set from the ViewModel after computing statistics.
    /// </summary>
    public List<(string Label, float Value, Color Color)> Bars { get; set; } = [];

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
        // Ensure maxValue ≥ 1 so the shortest bar has a positive width.
        float maxValue = Bars.Max(b => b.Value);
        if (maxValue <= 0) maxValue = 1;

        float marginLeft   = 70f;
        float marginRight  = 16f;
        float marginTop    = 12f;
        float marginBottom = 12f;

        float chartLeft    = marginLeft;
        float chartRight   = dirtyRect.Width - marginRight;
        float chartWidth   = chartRight - chartLeft;
        float barAreaHeight = dirtyRect.Height - marginTop - marginBottom;

        // Dynamically size bars so they fit with equal spacing.
        float barHeight  = Math.Min(28f, barAreaHeight / Bars.Count - 8f);
        float barSpacing = (barAreaHeight - barHeight * Bars.Count) / (Bars.Count + 1);

        float y = marginTop + barSpacing;

        // ── Render each bar ──────────────────────────
        foreach (var bar in Bars)
        {
            float barWidth = bar.Value / maxValue * chartWidth;
            if (barWidth < 3) barWidth = 3; // minimum visible width

            // Filled bar rectangle (4 px corner radius)
            canvas.FillColor = bar.Color;
            canvas.FillRoundedRectangle(chartLeft, y, barWidth, barHeight, 4);

            // Numeric value label right of the bar
            canvas.FontSize = 11;
            canvas.FontColor = Color.FromArgb("#555555");
            canvas.DrawString(
                $"{bar.Value:F0}",
                chartLeft + barWidth + 6, y, 50, barHeight,
                HorizontalAlignment.Left, VerticalAlignment.Center);

            // Category label (e.g. "3⭐  (2)") left of the bar
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

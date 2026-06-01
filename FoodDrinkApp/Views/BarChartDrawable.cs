namespace FoodDrinkApp.Views;

/// <summary>
/// Draws a horizontal bar chart for category-level statistics
/// (e.g. average calories per rating group).
/// Rendered by MAUI GraphicsView — zero external dependencies.
/// </summary>
public sealed class BarChartDrawable : IDrawable
{
    /// <summary>
    /// List of (label, value, colour) entries to render as bars.
    /// </summary>
    public List<(string Label, float Value, Color Color)> Bars { get; set; } = [];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;

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

        float maxValue = Bars.Max(b => b.Value);
        if (maxValue <= 0) maxValue = 1;

        float marginLeft = 70f;
        float marginRight = 16f;
        float marginTop = 12f;
        float marginBottom = 12f;

        float chartLeft = marginLeft;
        float chartRight = dirtyRect.Width - marginRight;
        float chartWidth = chartRight - chartLeft;
        float barAreaHeight = dirtyRect.Height - marginTop - marginBottom;
        float barHeight = Math.Min(28f, barAreaHeight / Bars.Count - 8f);
        float barSpacing = (barAreaHeight - barHeight * Bars.Count) / (Bars.Count + 1);

        float y = marginTop + barSpacing;

        foreach (var bar in Bars)
        {
            float barWidth = bar.Value / maxValue * chartWidth;
            if (barWidth < 3) barWidth = 3;

            // Bar
            canvas.FillColor = bar.Color;
            canvas.FillRoundedRectangle(chartLeft, y, barWidth, barHeight, 4);

            // Value label on the right of the bar
            canvas.FontSize = 11;
            canvas.FontColor = Color.FromArgb("#555555");
            canvas.DrawString(
                $"{bar.Value:F0}",
                chartLeft + barWidth + 6, y, 50, barHeight,
                HorizontalAlignment.Left, VerticalAlignment.Center);

            // Category label on the left
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

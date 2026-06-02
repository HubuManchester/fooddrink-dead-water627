namespace FoodDrinkApp.Views;

/// <summary>
/// Draws a donut chart showing macronutrient (Protein / Carbs / Fat) ratios
/// using <see cref="Microsoft.Maui.Graphics"/>.  When <see cref="GoalCalories"/>
/// is set the centre label displays progress toward the daily calorie target.
/// </summary>
public sealed class DonutChartDrawable : IDrawable
{
    /// <summary>Grams of protein across all entries.</summary>
    public float Protein { get; set; }

    /// <summary>Grams of carbohydrates across all entries.</summary>
    public float Carbs { get; set; }

    /// <summary>Grams of fat across all entries.</summary>
    public float Fat { get; set; }

    /// <summary>
    /// Daily calorie goal from user preferences (linked from Settings).
    /// When greater than zero the centre label changes from a plain kcal
    /// count to a progress indicator (e.g. "1 200 / 2 000 kcal").
    /// </summary>
    public int GoalCalories { get; set; }

    /// <summary>
    /// Total calories consumed from the current data set.
    /// Used alongside <see cref="GoalCalories"/> for goal progress display.
    /// </summary>
    public int TotalCaloriesConsumed { get; set; }

    // ── Colour palette ────────────────────────────────
    private static readonly Color ProteinColor = Color.FromArgb("#FF6384");
    private static readonly Color CarbsColor   = Color.FromArgb("#FFCE56");
    private static readonly Color FatColor     = Color.FromArgb("#36A2EB");

    /// <inheritdoc />
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float total = Protein + Carbs + Fat;

        canvas.Antialias = true;

        // ── Geometry ─────────────────────────────────
        float cx          = dirtyRect.Width / 2f;
        float cy          = dirtyRect.Height / 2f;
        float outerRadius = Math.Min(cx, cy) - 10f;
        float innerRadius = outerRadius * 0.55f;
        float lineWidth   = outerRadius - innerRadius;

        // ── Empty-state fallback ─────────────────────
        if (total <= 0)
        {
            canvas.StrokeColor = Color.FromArgb("#E0E0E0");
            canvas.StrokeSize  = lineWidth;
            canvas.DrawCircle(cx, cy, (outerRadius + innerRadius) / 2f);
            canvas.StrokeColor = null;

            canvas.Font      = new Microsoft.Maui.Graphics.Font("OpenSansRegular");
            canvas.FontSize  = 15;
            canvas.FontColor = Color.FromArgb("#9E9E9E");
            canvas.DrawString(
                "No nutrition data",
                cx - 60, cy - 10, 120, 20,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            return;
        }

        // ── Slices ───────────────────────────────────
        (float Value, Color Color, string Label)[] slices =
        [
            (Protein, ProteinColor, "Protein"),
            (Carbs,   CarbsColor,   "Carbs"),
            (Fat,     FatColor,     "Fat"),
        ];

        float startAngle = -90f;
        foreach (var slice in slices)
        {
            if (slice.Value <= 0) continue;

            float sweep    = slice.Value / total * 360f;
            float midAngle = startAngle + sweep / 2f;

            canvas.StrokeColor   = slice.Color;
            canvas.StrokeSize    = lineWidth;
            canvas.StrokeLineCap = LineCap.Butt;

            var path        = new PathF();
            float startRad  = startAngle * MathF.PI / 180f;
            float midR      = (outerRadius + innerRadius) / 2f;

            int segments = Math.Max(8, (int)(sweep / 2));
            path.MoveTo(
                cx + midR * MathF.Cos(startRad),
                cy + midR * MathF.Sin(startRad));

            for (int i = 1; i <= segments; i++)
            {
                float t = startRad + (sweep * MathF.PI / 180f) * i / segments;
                path.LineTo(
                    cx + midR * MathF.Cos(t),
                    cy + midR * MathF.Sin(t));
            }

            canvas.DrawPath(path);

            float pct     = slice.Value / total * 100f;
            float labelR  = outerRadius + 18f;
            float lx      = cx + labelR * MathF.Cos(midAngle * MathF.PI / 180f);
            float ly      = cy + labelR * MathF.Sin(midAngle * MathF.PI / 180f);

            canvas.FontSize  = 11;
            canvas.FontColor = slice.Color;
            canvas.DrawString(
                $"{pct:F0}%",
                lx - 22, ly - 8, 44, 16,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            startAngle += sweep;
        }

        // ── Centre label ─────────────────────────────
        if (GoalCalories > 0)
        {
            // Show goal-aware label: consumed / goal
            canvas.FontSize  = 11;
            canvas.FontColor = Color.FromArgb("#888888");
            canvas.DrawString(
                $"{TotalCaloriesConsumed} / {GoalCalories}",
                cx - 55, cy - 14, 110, 16,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.FontSize  = 13;
            canvas.FontColor = Color.FromArgb("#F29B38");
            canvas.DrawString(
                "kcal goal",
                cx - 55, cy + 4, 110, 16,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }
        else
        {
            canvas.FontSize  = 13;
            canvas.FontColor = Color.FromArgb("#666666");
            canvas.DrawString(
                $"{total:F0} kcal",
                cx - 50, cy - 9, 100, 18,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        // ── Bottom legend ────────────────────────────
        float legendY = dirtyRect.Height - 26f;
        float legendX = cx - 130f;
        DrawLegendCell(canvas, legendX,         legendY, ProteinColor, $"Protein  {Protein:F0}g");
        DrawLegendCell(canvas, legendX + 90f,   legendY, CarbsColor,   $"Carbs  {Carbs:F0}g");
        DrawLegendCell(canvas, legendX + 180f,  legendY, FatColor,     $"Fat  {Fat:F0}g");
    }

    /// <summary>Renders one legend colour swatch + label.</summary>
    private static void DrawLegendCell(ICanvas canvas, float x, float y, Color color, string text)
    {
        canvas.FillColor = color;
        canvas.FillRectangle(x, y, 10, 10);
        canvas.FontSize  = 10;
        canvas.FontColor = Color.FromArgb("#888888");
        canvas.DrawString(text, x + 14, y - 1, 80, 12,
            HorizontalAlignment.Left, VerticalAlignment.Center);
    }
}

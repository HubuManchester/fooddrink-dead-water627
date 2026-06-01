namespace FoodDrinkApp.Views;

/// <summary>
/// Draws a donut chart showing macronutrient (Protein / Carbs / Fat) ratios
/// using <see cref="Microsoft.Maui.Graphics"/>.  Rendered directly by MAUI
/// <c>GraphicsView</c> — zero external chart-library dependencies.
/// </summary>
/// <remarks>
/// <para>Visual structure (clockwise from 12-o'clock):</para>
/// <list type="bullet">
///   <item>Protein (pink, #FF6384) → Carbs (yellow, #FFCE56) → Fat (blue, #36A2EB)</item>
///   <item>Each arc segment is approximated with small line-segment paths.</item>
///   <item>Percentage labels hover outside each arc at the segment midpoint.</item>
///   <item>Centre text shows total kcal; a three-colour legend sits at the bottom.</item>
///   <item>When no nutrition data exists a grey placeholder ring is drawn.</item>
/// </list>
/// </remarks>
public sealed class DonutChartDrawable : IDrawable
{
    /// <summary>Grams of protein across all entries.</summary>
    public float Protein { get; set; }

    /// <summary>Grams of carbohydrates across all entries.</summary>
    public float Carbs { get; set; }

    /// <summary>Grams of fat across all entries.</summary>
    public float Fat { get; set; }

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
        float innerRadius = outerRadius * 0.55f;    // donut hole — 55 % of outer
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

        float startAngle = -90f; // 12-o'clock position
        foreach (var slice in slices)
        {
            if (slice.Value <= 0) continue;

            float sweep    = slice.Value / total * 360f;
            float midAngle = startAngle + sweep / 2f;

            // Thick arc segment via path approximation
            canvas.StrokeColor   = slice.Color;
            canvas.StrokeSize    = lineWidth;
            canvas.StrokeLineCap = LineCap.Butt;

            var path        = new PathF();
            float startRad  = startAngle * MathF.PI / 180f;
            float midR      = (outerRadius + innerRadius) / 2f;

            // Approximate the arc with N small line segments for smoothness
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

            // Percentage label at the arc midpoint, outside the ring
            float pct     = slice.Value / total * 100f;
            float labelR  = outerRadius + 18f;   // label radius
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

        // ── Centre label (total kcal) ────────────────
        canvas.FontSize  = 13;
        canvas.FontColor = Color.FromArgb("#666666");
        canvas.DrawString(
            $"{total:F0} kcal",
            cx - 50, cy - 9, 100, 18,
            HorizontalAlignment.Center, VerticalAlignment.Center);

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

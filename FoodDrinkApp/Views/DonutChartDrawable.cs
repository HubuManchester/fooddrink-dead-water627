namespace FoodDrinkApp.Views;

/// <summary>
/// Draws a donut chart showing macronutrient (Protein / Carbs / Fat) ratios.
/// Rendered by MAUI GraphicsView — zero external dependencies.
/// </summary>
public sealed class DonutChartDrawable : IDrawable
{
    public float Protein { get; set; }
    public float Carbs { get; set; }
    public float Fat { get; set; }

    private static readonly Color ProteinColor = Color.FromArgb("#FF6384");
    private static readonly Color CarbsColor   = Color.FromArgb("#FFCE56");
    private static readonly Color FatColor     = Color.FromArgb("#36A2EB");

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float total = Protein + Carbs + Fat;

        canvas.Antialias = true;
        float cx = dirtyRect.Width / 2f;
        float cy = dirtyRect.Height / 2f;
        float outerRadius = Math.Min(cx, cy) - 10f;
        float innerRadius = outerRadius * 0.55f; // donut hole
        float lineWidth = outerRadius - innerRadius;

        if (total <= 0)
        {
            // Draw grey placeholder ring + "No data" text
            canvas.StrokeColor = Color.FromArgb("#E0E0E0");
            canvas.StrokeSize = lineWidth;
            canvas.DrawCircle(cx, cy, (outerRadius + innerRadius) / 2f);
            canvas.StrokeColor = null;

            canvas.Font = new Microsoft.Maui.Graphics.Font("OpenSansRegular");
            canvas.FontSize = 15;
            canvas.FontColor = Color.FromArgb("#9E9E9E");
            canvas.DrawString(
                "No nutrition data",
                cx - 60, cy - 10, 120, 20,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            return;
        }

        // ── Draw slices ─────────────────────────────
        var slices = new[]
        {
            (Value: Protein, Color: ProteinColor, Label: "Protein"),
            (Value: Carbs,   Color: CarbsColor,   Label: "Carbs"),
            (Value: Fat,     Color: FatColor,     Label: "Fat"),
        };

        float startAngle = -90; // start from 12-o'clock
        foreach (var slice in slices)
        {
            if (slice.Value <= 0) continue;

            float sweep = slice.Value / total * 360f;
            float midAngle = startAngle + sweep / 2f;

            // Draw arc segment manually with thick stroke
            canvas.StrokeColor = slice.Color;
            canvas.StrokeSize = lineWidth;
            canvas.StrokeLineCap = LineCap.Butt;

            // Use path to draw arc
            var path = new PathF();
            float startRad = startAngle * MathF.PI / 180f;
            float endRad = (startAngle + sweep) * MathF.PI / 180f;
            float midR = (outerRadius + innerRadius) / 2f;

            // Approximate arc with many small line segments
            int segments = Math.Max(8, (int)(sweep / 2));
            float sx = cx + midR * MathF.Cos(startRad);
            float sy = cy + midR * MathF.Sin(startRad);
            path.MoveTo(sx, sy);

            for (int i = 1; i <= segments; i++)
            {
                float t = startRad + (sweep * MathF.PI / 180f) * i / segments;
                path.LineTo(cx + midR * MathF.Cos(t), cy + midR * MathF.Sin(t));
            }

            canvas.DrawPath(path);

            // Percentage label at midpoint of arc
            float pct = slice.Value / total * 100f;
            float labelR = outerRadius + 18f;
            float lx = cx + labelR * MathF.Cos(midAngle * MathF.PI / 180f);
            float ly = cy + labelR * MathF.Sin(midAngle * MathF.PI / 180f);

            canvas.FontSize = 11;
            canvas.FontColor = slice.Color;
            canvas.DrawString(
                $"{pct:F0}%",
                lx - 22, ly - 8, 44, 16,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            startAngle += sweep;
        }

        // ── Centre label ────────────────────────────
        canvas.FontSize = 13;
        canvas.FontColor = Color.FromArgb("#666666");
        canvas.DrawString(
            $"{total:F0} kcal",
            cx - 50, cy - 9, 100, 18,
            HorizontalAlignment.Center, VerticalAlignment.Center);

        // ── Legend ──────────────────────────────────
        float legendY = dirtyRect.Height - 26f;
        float legendX = cx - 130f;
        DrawLegendItem(canvas, legendX, legendY, ProteinColor, $"Protein  {Protein:F0}g");
        DrawLegendItem(canvas, legendX + 90f, legendY, CarbsColor, $"Carbs  {Carbs:F0}g");
        DrawLegendItem(canvas, legendX + 180f, legendY, FatColor, $"Fat  {Fat:F0}g");
    }

    private static void DrawLegendItem(ICanvas canvas, float x, float y, Color color, string text)
    {
        canvas.FillColor = color;
        canvas.FillRectangle(x, y, 10, 10);
        canvas.FontSize = 10;
        canvas.FontColor = Color.FromArgb("#888888");
        canvas.DrawString(text, x + 14, y - 1, 80, 12, HorizontalAlignment.Left, VerticalAlignment.Center);
    }
}

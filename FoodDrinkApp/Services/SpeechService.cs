namespace FoodDrinkApp.Services;

public static class SpeechService
{
    private static CancellationTokenSource? currentSpeech;

    public static async Task<bool> SpeakAsync(string text)
    {
        Stop();

        currentSpeech = new CancellationTokenSource();
        var options = new SpeechOptions
        {
            Volume = 0.9f,
            Pitch = 1.05f,
            Locale = await FindEnglishLocaleAsync()
        };

        try
        {
            await TextToSpeech.Default.SpeakAsync(text, options, currentSpeech.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            // Cancelled by user — not a failure.
            return true;
        }
        catch (FeatureNotSupportedException)
        {
            // Device or emulator without TTS engine — silent fallback.
            return false;
        }
        catch (Exception)
        {
            // Engine lag, unavailable locale, or any other runtime TTS fault.
            // Silently fall back so the user experience is never interrupted.
            return false;
        }
    }

    public static Task SpeakChineseAsync(string text) => SpeakAsync(text);

    public static void Stop()
    {
        if (currentSpeech is null)
        {
            return;
        }

        currentSpeech.Cancel();
        currentSpeech.Dispose();
        currentSpeech = null;
    }

    private static async Task<Locale?> FindEnglishLocaleAsync()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        return locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    }
}

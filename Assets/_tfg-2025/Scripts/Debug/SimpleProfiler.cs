using System.Diagnostics;

public class SimpleProfiler
{
    Stopwatch stopwatch = new Stopwatch();

    public float lastMs;
    public float averageMs;

    const int SAMPLE_WINDOW = 60;
    float[] samples = new float[SAMPLE_WINDOW];
    int index = 0;

    public void Begin()
    {
        stopwatch.Restart();
    }

    public void End()
    {
        stopwatch.Stop();
        lastMs = (float)stopwatch.Elapsed.TotalMilliseconds;

        samples[index] = lastMs;
        index = (index + 1) % SAMPLE_WINDOW;

        float sum = 0f;
        for (int i = 0; i < SAMPLE_WINDOW; i++)
            sum += samples[i];

        averageMs = sum / SAMPLE_WINDOW;
    }
}
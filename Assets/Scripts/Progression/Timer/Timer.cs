public struct Timer
{
    public float elapsed;
    public bool running;
    
    public void Tick(float deltaTime)
    {
        if (running)
        {
            elapsed += deltaTime;
        }
    }
    
    public void Reset()
    {
        elapsed = 0;
    }
}

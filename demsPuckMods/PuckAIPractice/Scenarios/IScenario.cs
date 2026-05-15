namespace PuckAIPractice.Scenarios
{
    public interface IScenario
    {
        string Name { get; }
        void Start(ulong callerClientId, string[] args);
        void Tick(float dt);
        void Stop();
    }
}

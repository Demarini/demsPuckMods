namespace PuckAIPractice.Scenarios
{
    public interface IScenario
    {
        string Name { get; }
        void Start(ulong callerClientId);
        void Stop();
    }
}

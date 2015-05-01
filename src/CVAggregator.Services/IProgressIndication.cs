namespace CVAggregator.Services
{
    public interface IProgressIndication
    {
        void Indeterminate(string message);
        void Progress(int current, int max, string message);
    }
}
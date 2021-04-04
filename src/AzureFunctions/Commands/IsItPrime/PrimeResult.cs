namespace AzureFunctions.Commands.IsItPrime
{
    public class PrimeResult
    {
        public long Number { get; set; }
        public bool IsPrime { get; set; }
        public string ExecutionTime { get; set; }
        public bool IsCached { get; set; }

    }
}
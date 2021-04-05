namespace AzureFunctions.Configuration.Options
{
    public class CosmosOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
    }
}
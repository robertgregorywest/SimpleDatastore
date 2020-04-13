namespace SimpleDatastore.Interfaces
{
    public interface IConfiguration
    {
        int CacheDuration { get; set; }
        bool EnableCaching { get; set; }
        string DatastoreLocation { get; set; }
    }
}

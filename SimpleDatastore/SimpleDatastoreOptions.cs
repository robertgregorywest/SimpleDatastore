namespace SimpleDatastore
{
    public class SimpleDatastoreOptions
    {
        public bool EnableCaching { get; set; } = true;
        public int CacheDuration { get; set; } = 60;
        public string DatastoreLocation { get; set; } = Constants.DataFolder;
    }
}

namespace SimpleDatastore
{
    public class SimpleDatastoreOptions
    {
        public int CacheDuration { get; set; } = 60;
        public bool EnableCaching { get; set; } = true;
        public string DatastoreLocation { get; set; } = Constants.DataFolder;
    }
}

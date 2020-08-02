namespace SimpleDatastore
{
    public class SimpleDatastoreOptions
    {
        public bool EnableCaching { get; set; } = true;
        public int CacheDuration { get; set; } = 60;
        public string DatastoreLocation { get; set; } = PersistentObject.DataFolder;

        public StorageModeOptions StorageMode { get; set; } = StorageModeOptions.Xml;

        public enum StorageModeOptions
        {
            Xml,
            Json
        }

        public bool PersistChildren { get; set; } = true;
    }
}

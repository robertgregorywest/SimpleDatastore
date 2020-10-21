namespace SimpleDatastore
{
    public class SimpleDatastoreOptions
    {
        /// <summary>
        /// Determines whether entities are cached in the memory cache
        /// </summary>
        public bool EnableCaching { get; set; } = true;
        
        /// <summary>
        /// Duration in minutes for which entities will be cached
        /// </summary>
        public int CacheDuration { get; set; } = 60;
        
        /// <summary>
        /// The directory on the file system where storage documents will be saved
        /// </summary>
        public string DatastoreLocation { get; set; } = PersistentObject.DataFolder;

        /// <summary>
        /// The type of storage format to use, either XML (default) or JSON
        /// </summary>
        public StorageModeOptions StorageMode { get; set; } = StorageModeOptions.Xml;

        public enum StorageModeOptions
        {
            Xml,
            Json
        }

        /// <summary>
        /// If true then child entities are stored in the relevant storage document for the type
        /// and their identities are stored and used to retrieve them. When set to false
        /// child entities are serialized as part of the parent
        /// </summary>
        public bool PersistChildren { get; set; } = true;
    }
}

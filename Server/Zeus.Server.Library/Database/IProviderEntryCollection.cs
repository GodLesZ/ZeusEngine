namespace Zeus.Server.Library.Database {

    public interface IProviderEntryCollection {

        /// <summary>
        /// Retruns true if the given record id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool RecordExists(long id);

    }

}
namespace Zeus.Server.Library.Database {

	public enum ConnectionError {

		/// <summary>
		/// No Error, all went fine
		/// </summary>
		None = 0,

		/// <summary>
		/// Connection was not Instanced
		/// </summary>
		OpenBeforeInit,
		/// <summary>
		/// catched Exception, Invalid Connection Info or something else
		/// </summary>
		CanNotConnectToDb,
		/// <summary>
		/// No Exception, but cant open SQL Connection
		/// </summary>
		FailedToOpen,
		/// <summary>
		/// Catched Exception, unkown thing while trying to open the connection
		/// </summary>
		UnknownOpen,

		/// <summary>
		/// Connection was not Instanced
		/// </summary>
		CloseBeforeInit,
		/// <summary>
        /// catched Exception, unkown thing while trying to open the connection
		/// </summary>
		CanNotDisconnectFromDb,
		/// <summary>
		/// No Exception, but cant close SQL Connection
		/// </summary>
		FailedToClose,
		/// <summary>
		/// Catched Exception, unkown thing while trying to close the connection
		/// </summary>
		UnknownClose,
	}

}

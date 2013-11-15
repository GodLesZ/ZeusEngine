using System;

namespace Zeus.Client.Library.Format.Ragnarok.Grf {

	[Flags()]
	public enum FileItemState {
		None = 0,
		Added = 1,
		Updated = 2,
		Deleted = 4
	}

}
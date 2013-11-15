
namespace Zeus.Client.Library.Format.Ragnarok.Grf {

	public delegate void RoGrfFileItemBeforeAddHandler();
	public delegate void RoGrfFileItemAddedHandler(FileItem item, int percent);
	public delegate void RoGrfFileItemBeforeWriteHandler();
	public delegate void RoGrfFileItemWriteHandler(FileItem item, int percent);
	public delegate void RoGrfProgressUpdateHandler(int progress);

}

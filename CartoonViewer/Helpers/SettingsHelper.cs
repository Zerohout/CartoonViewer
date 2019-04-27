namespace CartoonViewer.Helpers
{
	using System;

	public static class SettingsHelper
	{
		public static string AppPath = $"{AppDomain.CurrentDomain.BaseDirectory}";
		public const string AppDataFolderName = "AppData";
		public static readonly string AppDataPath = $"{AppPath}{AppDataFolderName}";
		public const string DefaultGeneralSettingsFileName = "DefaultGeneralSettingsValues";
		public const string SavedGeneralSettingsFileName = "SavedGeneralSettingsValues";
		public const string DefaultFilesExtension = ".cview";
		public const string NewElementString = "**Добавить новый**";
		public const string FreehatWebSite = "http://freehat.cc";
		public static (int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) GlobalIdList;
	}
}

namespace CartoonViewer.Helpers
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Windows;
	using Caliburn.Micro;
	using Models.SettingModels;
	using Newtonsoft.Json;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using static ClassWriterReader;

	public static class Helper
	{
		public static string AppPath = $"{AppDomain.CurrentDomain.BaseDirectory}";
		public const string MMBackgroundUri = "../../Resources/Images/HDMMBackground.png";
		public const string MMBackgroundOnExitUri = "../../Resources/Images/HDMMBackgroundOnExit.png";
		public const string AppDataFolderName = "AppData";
		public static readonly string AppDataPath = $"{AppPath}{AppDataFolderName}";
		public const string DefaultFilesExtension = ".cview";
		public const string DefaultGeneralSettingsFileName = "DefaultGeneralSettingsValues";
		public const string SavedGeneralSettingsFileName = "SavedGeneralSettingsValues";
		public const string NewElementString = "**Добавить новый**";
		public const string FreehatWebSite = "http://freehat.cc";

		public static int CurrentSkipCount = 6;
		public static int DelayedSkipCount = 5;
		public static (int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) GlobalIdList;

		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_CHAR = 0x102;
		public const int WM_SYSKEYDOWN = 0x104;
		public const int WM_SYSKEYUP = 0x105;
		public const int VK_LEFT = 0x25;
		public const int VK_RIGHT = 0x27;
		public const int VK_F = 0x46;
		public const int VK_ESCAPE = 0x1B;
		public const int VK_SPACE = 0x20;

		public static TimeSpan ApproximateEpisodeDuration = new TimeSpan(0, 21, 10);
		public static WindowManager WinMan = new WindowManager();
		public static MessageHelper Msg = new MessageHelper();
		public static Stopwatch Timer = new Stopwatch();
		public static Visibility AdvancedSettingsVisibility = Visibility.Hidden;

		public static IWebDriver Browser;
		public static IntPtr HWND;
		public static HotkeysRegistrator HotReg;

		public readonly static BindableCollection<string> CartoonTypes = new BindableCollection<string>
		{
			"/disney",
			"/pixar",
			"/serii",
			"/multfilm",
		};


		public enum DialogState
		{
			YES_NO,
			OK,
			YES_NO_CANCEL
		}

		public enum DialogType
		{
			SAVE_CHANGES,
			CANCEL_CHANGES,
			REMOVE_OBJECT,
			OVERWRITE_FILE,
			INFO
		}

		public enum DialogResult
		{
			YES_ACTION,
			NO_ACTION,
			CANCEL_ACTION

		}

		/// <summary>
		/// Запуск браузера
		/// </summary>
		public static void StartBrowser()
		{
			Browser = new ChromeDriver(AppDataPath);

			Thread.Sleep(1000);

			HWND = Msg.getWindowId(null,
								   Process.GetProcessesByName("chrome")
										  .Single(p => p.MainWindowTitle.Contains("Google"))
										  .MainWindowTitle);

			Browser.Manage().Window.Maximize();
		}

		/// <summary>
		/// Сравнение 2 объектов и возврат значения их равенства
		/// </summary>
		/// <param name="object1">1-й сравниваемый объект</param>
		/// <param name="object2">2-й сравниваемый объект</param>
		/// <returns></returns>
		public static bool IsEquals(object object1, object object2)
		{
			var val1 = JsonConvert.SerializeObject(object1);
			var val2 = JsonConvert.SerializeObject(object2);

			return Equals(val1, val2);
		}

		public static GeneralSettingsValue LoadGeneralSettings()
		{
			var fullDefaultSettingsFileName = $"{DefaultGeneralSettingsFileName}{DefaultFilesExtension}";
			var fullSavedSettingsFileName = $"{SavedGeneralSettingsFileName}{DefaultFilesExtension}";

			if(File.Exists($"{AppDataPath}\\{fullDefaultSettingsFileName}") is false)
			{
				WriteClassInFile(
					new GeneralSettingsValue(),
					DefaultGeneralSettingsFileName,
					DefaultFilesExtension,
					AppDataPath);
			}

			return ReadClassFromFile<GeneralSettingsValue>(
				File.Exists($"{AppDataPath}\\{fullSavedSettingsFileName}") is false
					? $"{AppDataPath}\\{fullDefaultSettingsFileName}"
					: $"{AppDataPath}\\{fullSavedSettingsFileName}");
		}

	}
}

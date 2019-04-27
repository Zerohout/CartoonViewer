namespace CartoonViewer.Helpers
{
	using System;
	using System.Collections.Generic;
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
		public const string MMBackgroundUri = "../../Resources/Images/HDMMBackground.png";
		public const string MMBackgroundOnExitUri = "../../Resources/Images/HDMMBackgroundOnExit.png";
		
		public static TimeSpan ApproximateEpisodeDuration = new TimeSpan(0, 21, 10);
		public static WindowManager WinMan = new WindowManager();
		public static MessageHelper Msg = new MessageHelper();
		public static Stopwatch Timer = new Stopwatch();

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
			Browser = new ChromeDriver(SettingsHelper.AppDataPath);

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
			var fullDefaultSettingsFileName = $"{SettingsHelper.DefaultGeneralSettingsFileName}{SettingsHelper.DefaultFilesExtension}";
			var fullSavedSettingsFileName = $"{SettingsHelper.SavedGeneralSettingsFileName}{SettingsHelper.DefaultFilesExtension}";

			if(File.Exists($"{SettingsHelper.AppDataPath}\\{fullDefaultSettingsFileName}") is false)
			{
				WriteClassInFile(
					new GeneralSettingsValue(), SettingsHelper.DefaultGeneralSettingsFileName, SettingsHelper.DefaultFilesExtension, SettingsHelper.AppDataPath);
			}

			return ReadClassFromFile<GeneralSettingsValue>(
				File.Exists($"{SettingsHelper.AppDataPath}\\{fullSavedSettingsFileName}") is false
					? $"{SettingsHelper.AppDataPath}\\{fullDefaultSettingsFileName}"
					: $"{SettingsHelper.AppDataPath}\\{fullSavedSettingsFileName}");
		}
	}
}

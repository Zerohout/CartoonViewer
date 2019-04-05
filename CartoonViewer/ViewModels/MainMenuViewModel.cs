namespace CartoonViewer.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Windows;
	using System.Windows.Input;
	using Database;
	using Helpers;
	using Models;
	using OpenQA.Selenium;
	using static Helpers.Helper;
	using Keys = System.Windows.Forms.Keys;
	using Screen = Caliburn.Micro.Screen;
	public partial class MainMenuViewModel : Screen
	{
		public MainMenuViewModel(HotkeysRegistrator hotReg)
		{
			hotReg.RegisterGlobalHotkey(Pause, Keys.Pause, ModifierKeys.None);
			hotReg.RegisterGlobalHotkey(Exit, Keys.Pause, ModifierKeys.Shift);
			hotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
			hotReg.RegisterGlobalHotkey(() => { SwitchEpisode = true; }, Keys.Right, ModifierKeys.Control);
		}

		public MainMenuViewModel()
		{

		}

		protected override void OnInitialize()
		{
			LoadCartoons();
			SetDefaultValues();

			base.OnInitialize();
		}
		
		/// <summary>
		/// Загрузка списка мультфильмов из базы данных
		/// </summary>
		private void LoadCartoons()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Load();
				Cartoons.AddRange(ctx.Cartoons.Local.OrderBy(c => c.CartoonId));
			}
		}
		
		#region Просмотр серий

		/// <summary>
		/// Метод начала просмотра серий
		/// </summary>
		private void StartBrowsing()
		{
			var rndCartList = new List<int>();

			//создание рандомного списка эпизодов
			for (var i = 0; i < int.Parse(EpisodeCount); i++)
			{
				rndCartList.Add(rnd.Next(1, 101) % CheckedCartoons.Count);
			}

			foreach (var cartoonNum in rndCartList)
			{
				PlayEpisode(CheckedCartoons[cartoonNum]);
			}

			if (ShutdownComp)
			{
				var psi = new ProcessStartInfo("shutdown", "/s /t 0")
				{
					CreateNoWindow = true,
					UseShellExecute = false
				};

				Process.Start(psi);
			}

			Exit();
		}

		private void PlayEpisode(Cartoon cartoon)
		{
			//цикл для переключения серии без потерь в количестве указанных просмотров
			do
			{
				SwitchEpisode = false;

				SetCartoonsSettings(cartoon);
				StartVideoPlayer();

				Helper.Timer.Restart();
				LaunchMonitoring();

			} while (SwitchEpisode);
		}

		/// <summary>
		/// Запуск серии в видео проигрывателе
		/// </summary>
		private void StartVideoPlayer()
		{
			WebElement = Browser.FindElement(By.CssSelector("pjsdiv:nth-child(8) > pjsdiv > pjsdiv"));

			WebElement.Click();
			Thread.Sleep(500);
			WebElement.Click();
			Thread.Sleep(500);

			Msg.PressKey(VK_F);
			Msg.PressKey(VK_LEFT);
			Msg.PressKey(VK_RIGHT, CurrentSkipCount);

			Thread.Sleep(500);
			Msg.PressKey(VK_SPACE);
		}

		#endregion

		#region Настройки мультфильмов

		/// <summary>
		/// Установка настроек мультфильмов
		/// </summary>
		/// <param name="cartoon">Мультфильм</param>
		private void SetCartoonsSettings(Cartoon cartoon)
		{
			var url = GetEpisodeUrl(cartoon);
			var episodeNum = ExtractNumber(url);

			switch (cartoon.Name)
			{
				case "Южный парк":
					SetSouthParkSettings(episodeNum, url);
					CurrentDuration = new TimeSpan(0, 21, 10);
					break;
				case "Гриффины":
					CurrentDuration = new TimeSpan(0, 21, 30);
					break;
			}
		}

		/// <summary>
		/// Настройка дополнительных значений в "Южном Парке"
		/// </summary>
		/// <param name="episodeNum">Номер серии</param>
		private void SetSouthParkSettings(int episodeNum, string url)
		{
			if (episodeNum >= 101 && episodeNum <= 113)
			{
				Browser.Navigate().GoToUrl($"{url}?v=par");
			}

			if (episodeNum == 514 || episodeNum == 405)
			{
				CurrentSkipCount = 0;
			}
			else if (episodeNum >= 801 &&
					 episodeNum <= 814)
			{
				CurrentSkipCount = 8;
			}
			else if (episodeNum == 1901 ||
					 episodeNum == 1902 ||
					 episodeNum == 1905 ||
					 episodeNum == 1906 ||
					 (episodeNum >= 2001 && episodeNum <= 2010))
			{
				CurrentSkipCount = 10;
			}
			else if (episodeNum == 1903 ||
					 episodeNum == 1904 ||
					 episodeNum == 1907 ||
					 episodeNum == 1908 ||
					 episodeNum == 1910 ||
					 episodeNum == 2104 ||
					 episodeNum == 2105)
			{
				CurrentSkipCount = 11;
			}
			else if (episodeNum == 1406 || episodeNum == 1909 ||
					 (episodeNum >= 2101 && episodeNum <= 2103))
			{
				CurrentSkipCount = 12;
			}
			else if ((episodeNum >= 2106 && episodeNum <= 2110) ||
					 (episodeNum >= 2202 && episodeNum <= 2209))
			{
				CurrentSkipCount = 13;
			}
			else if (episodeNum == 2201)
			{
				CurrentSkipCount = 14;
			}
			else if (episodeNum == 2210)
			{
				CurrentSkipCount = 15;
			}
			else
			{
				CurrentSkipCount = 7;
			}
		}

		#endregion

		#region Мониторинг

		/// <summary>
		/// Запуск таймера мониторинга
		/// </summary>
		private void LaunchMonitoring()
		{
			var autoEvent = new AutoResetEvent(false);

			var stateTimer = new Timer(MonitoringAction, autoEvent,
									   new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 1));
			autoEvent.WaitOne();
			autoEvent.Dispose();
			stateTimer.Dispose();
		}

		/// <summary>
		/// Действия мониторинга
		/// </summary>
		/// <param name="stateInfo"></param>
		public void MonitoringAction(object stateInfo)
		{
			var autoEvent = (AutoResetEvent)stateInfo;

			//Таймер превысил длительность серии
			if (Helper.Timer.Elapsed > CurrentDuration || SwitchEpisode)
			{
				Helper.Timer.Reset();
				Msg.PressKey(VK_ESCAPE);
				autoEvent.Set();
			}

			DelayedSkipMonitoring();
			PauseMonitoring();
		}

		/// <summary>
		/// Мониторинг отложенного запуска
		/// </summary>
		private void DelayedSkipMonitoring()
		{
			//Отложенный запуск закончил действие
			if (IsDelayedSkip && Helper.Timer.Elapsed > DelayedSkipDuration)
			{
				Msg.PressKey(VK_SPACE);
				Msg.PressKey(VK_RIGHT, DelayedSkipCount);
				Msg.PressKey(VK_SPACE);

				IsDelayedSkip = false;
				Helper.Timer.Restart();
			}
		}

		/// <summary>
		/// Мониторинг состояния паузы
		/// </summary>
		private void PauseMonitoring()
		{
			//Серия поставлена на паузу
			if (IsPause && Helper.Timer.IsRunning)
			{
				Helper.Timer.Stop();
				Msg.PressKey(VK_SPACE);
				((MainViewModel)Parent).WindowState = WindowState.Normal;
				return;
			}

			//Серия снята с паузы
			if (!IsPause && !Helper.Timer.IsRunning)
			{
				Helper.Timer.Start();
				((MainViewModel)Parent).WindowState = WindowState.Minimized;
				Msg.PressKey(VK_SPACE);
			}
		}

		#endregion
		
		#region Дополнительные методы

		/// <summary>
		/// Получить url серии
		/// </summary>
		/// <param name="cartoon"></param>
		/// <returns></returns>
		private string GetEpisodeUrl(Cartoon cartoon)
		{
			//Отсеивание нежелательных серий
			while (true)
			{
				//Попытки перейти на указанный url при нестабильном интернет соединении
				for (var i = 0; i < 10; i++)
				{
					try
					{
						Browser.Navigate().GoToUrl($"https://{cartoon.Url}.freehat.cc/episode/rand.php");
						break;
					}
					catch
					{
						Browser.Navigate().Refresh();
					}
				}

				var url = Browser.Url;

				if (ExtractNumber(url) > 0)
				{
					return url;
				}
			}
		}

		/// <summary>
		/// Извлечь номер серии из url серии
		/// </summary>
		/// <param name="address">url серии</param>
		/// <returns></returns>
		private int ExtractNumber(string address)
		{
			var temp = "";

			foreach (var s in address)
			{
				if (char.IsDigit(s))
				{
					temp += s;
				}
			}

			if (int.TryParse(temp, out var res))
			{
				if (res < 100)
				{
					return -1;
				}

				return res;
			}

			return -1;
		}
		#endregion

	}
}

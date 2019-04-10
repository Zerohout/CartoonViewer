namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices.ComTypes;
	using System.Threading;
	using System.Windows;
	using System.Windows.Input;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using OpenQA.Selenium;
	using Keys = System.Windows.Forms.Keys;
	using Screen = Caliburn.Micro.Screen;
	public partial class MainMenuViewModel : Screen
	{
		public MainMenuViewModel()
		{
			
		}

		protected override void OnInitialize()
		{
			Helper.HotReg.RegisterGlobalHotkey(() => { IsPause = !IsPause; }, Keys.Pause, ModifierKeys.None);
			Helper.HotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
			Helper.HotReg.RegisterGlobalHotkey(() => { IsSwitchEpisode = true; }, Keys.Right, ModifierKeys.Control);
			//Helper.HotReg.RegisterGlobalHotkey(() => EpisodeCountString = (EpisodeCount - 1).ToString(), Keys.Delete, ModifierKeys.Shift);

			LoadCartoons();

			base.OnInitialize();
		}

		/// <summary>
		/// Загрузка списка мультфильмов из базы данных
		/// </summary>
		private void LoadCartoons()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Include(c => c.CartoonUrls).Load();
				Cartoons.AddRange(ctx.Cartoons.Local.OrderBy(c => c.CartoonId));
			}

			CheckedValidation();
			NotifyOfPropertyChange(() => Cartoons);
		}

		#region Просмотр серий

		private void RemoveCartoonFromQueue(int count)
		{
			for (var i = 0; i < count; i++)
			{
				RandomCartoonNumberList.Remove(RandomCartoonNumberList.Last());
			}
		}

		private void AddCartoonToQueue(int count)
		{
			for (var i = 0; i < count; i++)
			{
				RandomCartoonNumberList.Add(rnd.Next(1, 101) % CheckedCartoons.Count);
			}
		}

		/// <summary>
		/// Метод начала просмотра серий
		/// </summary>
		private void StartWatch()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Where(c => c.Checked)
				   .Include(c => c.CartoonUrls
				                  .Where(cu => cu.Checked));
			}


			AddCartoonToQueue(EpisodeCount);

			EpisodesCountRemainingString = "Осталось серий";

			while (EpisodeCount > 0)
			{
				EpisodeCountString = (EpisodeCount - 1).ToString();

				//цикл для переключения серии без потерь в количестве указанных просмотров
				do
				{
					IsSwitchEpisode = false;

					PlayEpisode(CheckedCartoons[RandomCartoonNumberList[CurrentEpisodeIndex]]);

					if (IsSwitchEpisode)
					{
						AddCartoonToQueue(1);
					}
					
					CurrentEpisodeIndex++;


				} while (IsSwitchEpisode);

			}

			if (IsShutdownComp)
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
			Cartoon _cartoon;
			
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Where(c => c.CartoonId == cartoon.CartoonId).Include("CartoonUrl").Include("ElementValues").Load();
				_cartoon = ctx.Cartoons.Local.First(c => c.CartoonId == cartoon.CartoonId);
			}

			SetCartoonsSettings(_cartoon);
			//StartVideoPlayer(_cartoon.ElementValues.First(e => e.UserElementName.Contains("кнопка")));

			Helper.Timer.Restart();
			LaunchMonitoring();

		}

		/// <summary>
		/// Запуск серии в видео проигрывателе
		/// </summary>
		private void StartVideoPlayer(ElementValue elementValue)
		{
			WebElement = Helper.Browser.FindElement(By.CssSelector(elementValue.CssSelector));

			WebElement.Click();
			Thread.Sleep(500);
			WebElement.Click();
			Thread.Sleep(500);

			Helper.Msg.PressKey(Helper.VK_F);
			Helper.Msg.PressKey(Helper.VK_LEFT);
			Helper.Msg.PressKey(Helper.VK_RIGHT, Helper.CurrentSkipCount);

			Thread.Sleep(500);
			Helper.Msg.PressKey(Helper.VK_SPACE);
		}

		#endregion

		#region Настройки мультфильмов

		/// <summary>
		/// Установка настроек мультфильмов
		/// </summary>
		/// <param name="cartoon">Мультфильм</param>
		private void SetCartoonsSettings(Cartoon cartoon)
		{
			//if (cartoon.CartoonUrl.MainUrl.Contains("freehat.cc"))
			//{
			//	var url = OpenAndGetRandomEpisodeUrl(cartoon);
			//	var episodeNum = ExtractNumber(url);
			//	switch (cartoon.Name)
			//	{
			//		case "Южный парк":
			//			SetSouthParkSettings(episodeNum, url);
			//			CurrentDuration = new TimeSpan(0, 21, 10);
			//			break;
			//		case "Гриффины":
			//			SetFamilyGuySettings(episodeNum, url);
			//			CurrentDuration = new TimeSpan(0, 21, 30);
			//			break;
			//	}
			//}
		}

		/// <summary>
		/// Настройка дополнительных значений в "Южном Парке"
		/// </summary>
		/// <param name="episodeNum">Номер серии</param>
		/// <param name="url">Адрес серии</param>
		private void SetSouthParkSettings(int episodeNum, string url)
		{
			if (episodeNum >= 101 && episodeNum <= 113)
			{
				Helper.Browser.Navigate().GoToUrl($"{url}?v=par");
			}

			if (episodeNum == 514 || episodeNum == 405)
			{
				Helper.CurrentSkipCount = 0;
			}
			else if (episodeNum >= 801 &&
					 episodeNum <= 814)
			{
				Helper.CurrentSkipCount = 8;
			}
			else if (episodeNum == 1901 ||
					 episodeNum == 1902 ||
					 episodeNum == 1905 ||
					 episodeNum == 1906 ||
					 (episodeNum >= 2001 && episodeNum <= 2010))
			{
				Helper.CurrentSkipCount = 10;
			}
			else if (episodeNum == 1903 ||
					 episodeNum == 1904 ||
					 episodeNum == 1907 ||
					 episodeNum == 1908 ||
					 episodeNum == 1910 ||
					 episodeNum == 2104 ||
					 episodeNum == 2105 ||
					 (episodeNum >= 1803 &&
					  episodeNum <= 1806))
			{
				Helper.CurrentSkipCount = 11;
			}
			else if (episodeNum == 1406 || episodeNum == 1909 ||
					 (episodeNum >= 2101 && episodeNum <= 2103))
			{
				Helper.CurrentSkipCount = 12;
			}
			else if ((episodeNum >= 2106 && episodeNum <= 2110) ||
					 (episodeNum >= 2202 && episodeNum <= 2209) ||
					 episodeNum == 1802 || episodeNum == 1807 ||
					 episodeNum == 1809)
			{
				Helper.CurrentSkipCount = 13;
			}
			else if (episodeNum == 2201 ||
					 episodeNum == 1801 ||
					 episodeNum == 1808 ||
					 episodeNum == 1810)
			{
				Helper.CurrentSkipCount = 14;
			}
			else if (episodeNum == 2210)
			{
				Helper.CurrentSkipCount = 15;
			}
			else
			{
				Helper.CurrentSkipCount = 7;
			}
		}

		/// <summary>
		/// Настройка дополнительных значений в "Гриффинах"
		/// </summary>
		/// <param name="episodeNum">Номер серии</param>
		/// <param name="url">Адрес серии</param>
		private void SetFamilyGuySettings(int episodeNum, string url)
		{

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

			NotifyEpisodesTime();

			//Таймер превысил длительность серии
			if (ElapsedTime > CurrentDuration || IsSwitchEpisode)
			{
				Helper.Timer.Reset();
				Helper.Msg.PressKey(Helper.VK_ESCAPE);
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
				Helper.Timer.Stop();
				Helper.Msg.PressKey(Helper.VK_SPACE);
				Helper.Msg.PressKey(Helper.VK_RIGHT, Helper.DelayedSkipCount);
				Helper.Msg.PressKey(Helper.VK_SPACE);

				IsDelayedSkip = false;
				Helper.Timer.Start();
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
				Helper.Msg.PressKey(Helper.VK_SPACE);
				((MainViewModel)Parent).WindowState = WindowState.Normal;
				return;
			}

			//Серия снята с паузы
			if (!IsPause && !Helper.Timer.IsRunning)
			{
				Helper.Timer.Start();
				((MainViewModel)Parent).WindowState = WindowState.Minimized;
				Helper.Msg.PressKey(Helper.VK_SPACE);
			}
		}

		#endregion

		#region Дополнительные методы

		/// <summary>
		/// Перейти на адрес мультфильма и получить его адрес
		/// </summary>
		/// <param name="cartoon"></param>
		/// <returns></returns>
		private string OpenAndGetRandomEpisodeUrl(Cartoon cartoon)
		{
			//Отсеивание нежелательных серий
			while (true)
			{
				//Попытки перейти на указанный url при нестабильном интернет соединении
				for (var i = 0; i < 10; i++)
				{
					try
					{
						//Helper.Browser.Navigate().GoToUrl($"{cartoon.CartoonUrl.MainUrl}{cartoon.CartoonUrl.UrlParameter}");
						break;
					}
					catch
					{
						Helper.Browser.Navigate().Refresh();
					}
				}

				var url = Helper.Browser.Url;

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

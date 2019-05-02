// ReSharper disable CheckNamespace
namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices.ComTypes;
	using System.Threading;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Helpers;
	using Models.CartoonModels;
	using OpenQA.Selenium;
	using static Helpers.MessageHelper;
	using static Helpers.ClassWriterReader;
	using static Helpers.SettingsHelper;

	public partial class MainMenuViewModel : Screen
	{
		private int jumperCount = 0;

		private int CurrentOptionId = 0;
		public List<Jumper> Jumpers { get; set; } = new List<Jumper>();
		public Jumper CurrentJumper { get; set; }

		/// <summary>
		/// Загрузка списка мультсериалов из базы данных
		/// </summary>
		private void LoadCartoons()
		{
			Cartoons = new BindableCollection<Cartoon>(
				CvDbContext.Cartoons
								 .Include(c => c.CartoonUrls)
								 .Include(c => c.CartoonWebSites)
								 .Include(c => c.CartoonSeasons)
								 .ToList());

			CheckedValidation();
		}

		#region Просмотр серий


		public TimeSpan IntellectualShutdownTimer { get; set; }

		/// <summary>
		/// Метод начала просмотра серий
		/// </summary>
		private void StartWatch()
		{
			IntellectualShutdownTimer = new TimeSpan();
			EpisodesCountRemainingString = "Осталось серий";
			
			if(GeneralSettings.WatchingInRow is true)
			{
				CheckedEpisodes = CvDbContext.Cartoons
											 .Include(c => c.CartoonEpisodes)
											 .First(c => c.Checked)
											 .CartoonEpisodes.OrderBy(ce => ce.FullNumber).ToList();

				var number = GeneralSettings.LastWatchedEpisodeInRowFullNumber;
				var tempEpisode = CheckedEpisodes.First(ce => ce.FullNumber == number);
				CurrentEpisodeIndex = CheckedEpisodes.IndexOf(tempEpisode) + 1;

			}
			else
			{
				CheckedEpisodes = ShuffleEpisode(CheckedEpisodes, GeneralSettings.RandomMixCount ?? 1);
				CurrentEpisodeIndex = 0;
			}

			while(GeneralSettings.EpisodesCount > 0 && GeneralSettings.AvailableEpisodesCount > 0)
			{

				GeneralSettings.EpisodesCount--;
				NotifyOfPropertyChange(() => GeneralSettings);

				//цикл для переключения серии без потерь в количестве указанных просмотров
				do
				{
					IsSwitchEpisode = false;
					GeneralSettings.AvailableEpisodesCount--;
					TotalEpisodeTime = new TimeSpan();
					PlayEpisode(CheckedEpisodes[CurrentEpisodeIndex++]);

				} while(IsSwitchEpisode && GeneralSettings.AvailableEpisodesCount > 0);
			}

			if(IsShutdownComp)
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

		private void PlayEpisode(CartoonEpisode episode)
		{
			var cartUrl = episode.Cartoon.CartoonUrls.First(cu => cu.Checked);
			var webSite = CvDbContext.CartoonWebSites
									 .Include(cws => cws.ElementValues)
									 .First(cws => cws.CartoonWebSiteId ==
												   cartUrl.CartoonWebSiteId);

			var urlString =
				$"{cartUrl.Url}{episode.FullNumber}/{episode.CartoonVoiceOver.UrlParameter}";
			var elementValue = webSite.ElementValues.First();

			CurrentOptionId = episode.EpisodeOptions
									 .First(eo => eo.CartoonEpisodeId == episode.CartoonEpisodeId &&
												  eo.CartoonVoiceOverId ==
												  episode.CartoonVoiceOver.CartoonVoiceOverId).EpisodeOptionId;

			//Попытки перейти на указанный url при нестабильном интернет соединении
			for(var i = 0; i < 10; i++)
			{
				try
				{
					Helper.Browser.Navigate().GoToUrl(urlString);
					break;
				}
				catch
				{
					Helper.Browser.Navigate().Refresh();
				}
			}

			var option = CvDbContext.EpisodeOptions
									.Include(eo => eo.Jumpers)
									.First(eo => eo.EpisodeOptionId == CurrentOptionId);

			CurrentDuration = option.Duration;
			option.LastDateViewed = DateTime.Now;
			CvDbContext.SaveChanges();
			Jumpers = new List<Jumper>(option.Jumpers);
			CurrentJumper = Jumpers[jumperCount++];

			WebElement = Helper.Browser.FindElement(By.CssSelector(elementValue.CssSelector));
			StartVideoPlayer();

			Helper.Timer.Restart();
			LaunchMonitoring();
			if(GeneralSettings.WatchingInRow)
			{
				GeneralSettings.LastWatchedEpisodeInRowFullNumber = episode.FullNumber;
				WriteClassInFile(GeneralSettings,
								 SavedGeneralSettingsFileName,
								 GeneralSettingsFileExtension,
								 AppDataPath);
			}
		}



		/// <summary>
		/// Запуск серии в видео проигрывателе
		/// </summary>
		private void StartVideoPlayer()
		{
			WebElement.Click();
			Thread.Sleep(500);

			if(CurrentJumper.StartTime > new TimeSpan())
			{
				//IsDelayedSkip = true;
				Helper.Msg.PressKey(VK_F);
				return;
			}


			WebElement.Click();
			Thread.Sleep(500);

			Helper.Msg.PressKey(VK_F);

			Helper.Msg.PressKey(VK_LEFT);
			Helper.Msg.PressKey(VK_RIGHT, CurrentJumper.SkipCount);
			TotalEpisodeTime += new TimeSpan(0, 0, CurrentJumper.SkipCount * 5);

			Thread.Sleep(500);
			Helper.Msg.PressKey(VK_SPACE);

			CurrentJumper = jumperCount < Jumpers.Count
				? Jumpers[jumperCount++]
				: null;
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
									   new TimeSpan(0, 0, 0),
									   new TimeSpan(0, 0, 1));
			autoEvent.WaitOne();
			autoEvent.Dispose();
			stateTimer.Dispose();
		}

		/// <summary>
		/// Действия мониторинга
		/// </summary>
		/// <param name="stateInfo"></param>
		private void MonitoringAction(object stateInfo)
		{
			var autoEvent = (AutoResetEvent)stateInfo;

			NotifyOfPropertyChange(() => GeneralSettings);
			NotifyOfPropertyChange(() => EndDate);
			NotifyOfPropertyChange(() => EndTime);

			PauseMonitoring();

			if(Helper.Timer.IsRunning)
			{
				if(CurrentJumper != null)
				{
					if(TotalEpisodeTime >= CurrentJumper.StartTime &&
						TotalEpisodeTime < CurrentJumper.EndTime)
					{
						DelayedSkipMonitoring();
					}
				}

				// Активация выключения компьютера, при включенном ночном помощнике
				if(GeneralSettings.NightHelperShutdown is true && IsShutdownComp is false)
				{
					if(IntellectualShutdownTimer >= GeneralSettings.NightHelperShutdownTimeSpan &&
						GeneralSettings.NightHelperShutdownReachedTime > DateTime.Now.TimeOfDay)
					{
						IsShutdownComp = true;
					}

					IntellectualShutdownTimer += new TimeSpan(0, 0, 1);
				}

				TotalEpisodeTime += new TimeSpan(0, 0, 1);
			}

			//Таймер превысил длительность серии
			if(ElapsedTime > CurrentDuration || IsSwitchEpisode)
			{
				if(IsPaused is true)
				{
					IsPaused = false;
					IsNowPause = false;
				}

				jumperCount = 0;
				Helper.Timer.Reset();
				Helper.Msg.PressKey(VK_ESCAPE);
				autoEvent.Set();
			}

			TotalEpisodeTime += new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// Действия после отложенного запуска
		/// </summary>
		private void DelayedSkipMonitoring()
		{
			TotalEpisodeTime += new TimeSpan(0, 0, CurrentJumper.SkipCount * 5);
			Helper.Timer.Stop();

			Helper.Msg.PressKey(VK_SPACE);
			Helper.Msg.PressKey(VK_RIGHT, CurrentJumper.SkipCount);
			Helper.Msg.PressKey(VK_SPACE);


			CurrentJumper = jumperCount < Jumpers.Count
				? Jumpers[jumperCount++]
				: null;

			Helper.Timer.Start();
		}


		/// <summary>
		/// Мониторинг состояния паузы
		/// </summary>
		private void PauseMonitoring()
		{
			//Серия поставлена на паузу
			if(IsPaused && Helper.Timer.IsRunning)
			{
				IsNowPause = true;
				Helper.Timer.Stop();
				Helper.Msg.PressKey(VK_SPACE);
				((MainViewModel)Parent).WindowState = WindowState.Normal;
				return;
			}

			//Серия снята с паузы
			if(!IsPaused && !Helper.Timer.IsRunning && IsNowPause)
			{
				Helper.Timer.Start();
				((MainViewModel)Parent).WindowState = WindowState.Minimized;
				Helper.Msg.PressKey(VK_SPACE);
				IsNowPause = false;
			}
		}

		#endregion

		#region Дополнительные методы

		private List<CartoonEpisode> ShuffleEpisode(List<CartoonEpisode> list, int count)
		{
			for(var i = 0; i < count; i++)
			{
				for(var j = list.Count - 1; j >= 1; j--)
				{
					var k = rnd.Next(j + 1);
					// обменять значения data[j] и data[i]
					var temp = list[k];
					list[k] = list[j];
					list[j] = temp;
				}
			}

			return list;
		}

		#endregion
	}
}

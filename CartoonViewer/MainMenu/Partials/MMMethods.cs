// ReSharper disable CheckNamespace
namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Helpers;
	using Models.CartoonModels;
	using OpenQA.Selenium;
	using static Helpers.MessageHelper;

	public partial class MainMenuViewModel : Screen
	{
		/// <summary>
		/// Загрузка списка мультсериалов из базы данных
		/// </summary>
		private async void LoadCartoons()
		{
			Cartoons = new BindableCollection<Cartoon>(
				await CvDbContext.Cartoons
								 .Include(c => c.CartoonUrls)
								 .Include(c => c.CartoonWebSites)
								 .Include(c => c.CartoonSeasons)
								 .ToListAsync());

			CheckedValidation();
		}

		#region Просмотр серий

		/// <summary>
		/// Метод начала просмотра серий
		/// </summary>
		private void StartWatch()
		{
			EpisodesCountRemainingString = "Осталось серий";

			CheckedEpisodes = ShuffleEpisode(CheckedEpisodes, GeneralSettings.RandomMixCount ?? 1);

			CheckedEpisodes[1] = CheckedEpisodes.First(ce => ce.DelayedSkip > new TimeSpan());

			CurrentEpisodeIndex = 0;

			while(GeneralSettings.EpisodesCount > 0)
			{
				GeneralSettings.EpisodesCount--;
				GeneralSettings.AvailableEpisodesCount--;
				NotifyOfPropertyChange(() => GeneralSettings);

				//цикл для переключения серии без потерь в количестве указанных просмотров
				do
				{
					IsSwitchEpisode = false;

					PlayEpisode(CheckedEpisodes[CurrentEpisodeIndex++]);

					if(IsSwitchEpisode)
					{
						if(GeneralSettings.EpisodesCount + CurrentEpisodeIndex != 
						   GeneralSettings.AvailableEpisodesCount)
						{
							GeneralSettings.EpisodesCount++;
						}
					}


				} while(IsSwitchEpisode);

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
				$"{cartUrl.Url}{episode.CartoonSeason.Number * 100 + episode.Number}/{episode.CartoonVoiceOver.UrlParameter}";
			var elementValue = webSite.ElementValues.First();

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

			CurrentDuration = episode.Duration;
			DelayedSkipDuration = episode.DelayedSkip;
			CurrentSkipCount = episode.SkipCount;
			episode.LastDateViewed = DateTime.Now;
			CvDbContext.SaveChanges();

			WebElement = Helper.Browser.FindElement(By.CssSelector(elementValue.CssSelector));
			StartVideoPlayer();

			Helper.Timer.Restart();
			LaunchMonitoring();
		}

		/// <summary>
		/// Запуск серии в видео проигрывателе
		/// </summary>
		private void StartVideoPlayer()
		{

			WebElement.Click();
			Thread.Sleep(500);


			if(DelayedSkipDuration > new TimeSpan())
			{
				IsDelayedSkip = true;
				Helper.Msg.PressKey(VK_F);
				return;
			}


			WebElement.Click();
			Thread.Sleep(500);

			Helper.Msg.PressKey(VK_F);

			Helper.Msg.PressKey(VK_LEFT);
			Helper.Msg.PressKey(VK_RIGHT, CurrentSkipCount);

			Thread.Sleep(500);
			Helper.Msg.PressKey(VK_SPACE);
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

			if(Helper.Timer.IsRunning && IsDelayedSkip &&
			   Helper.Timer.Elapsed >= DelayedSkipDuration)
			{
				DelayedSkipMonitoring();
			}

			//Таймер превысил длительность серии
			if(ElapsedTime > CurrentDuration || IsSwitchEpisode)
			{
				if (IsDelayedSkip is true) IsDelayedSkip = false;
				if(IsPaused is true)
				{
					IsPaused = false;
					IsNowPause = false;
				}

				Helper.Timer.Reset();
				Helper.Msg.PressKey(VK_ESCAPE);
				autoEvent.Set();
			}
		}

		/// <summary>
		/// Действия после отложенного запуска
		/// </summary>
		private void DelayedSkipMonitoring()
		{
			IsDelayedSkip = false;
			Helper.Timer.Stop();

			Helper.Msg.PressKey(VK_SPACE);
			Helper.Msg.PressKey(VK_RIGHT, CurrentSkipCount);
			Helper.Msg.PressKey(VK_SPACE);

			Helper.Timer.Start();
		}

		public int CurrentSkipCount { get; set; }

		public bool IsNowPause { get; set; }

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

		/// <summary>
		/// Перейти на адрес мультсериала и получить его адрес
		/// </summary>
		/// <param name="cartoon"></param>
		/// <returns></returns>
		private string OpenAndGetRandomEpisodeUrl(Cartoon cartoon)
		{
			//Отсеивание нежелательных серий
			while(true)
			{


				var url = Helper.Browser.Url;

				if(ExtractNumber(url) > 0)
				{
					return url;
				}
			}
		}

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

		/// <summary>
		/// Извлечь номер серии из url серии
		/// </summary>
		/// <param name="address">url серии</param>
		/// <returns></returns>
		private int ExtractNumber(string address)
		{
			var temp = "";

			foreach(var s in address)
			{
				if(char.IsDigit(s))
				{
					temp += s;
				}
			}

			if(int.TryParse(temp, out var res))
			{
				if(res < 100)
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

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

		private void RemoveCartoonFromQueue(int count)
		{
			for(var i = 0; i < count; i++)
			{
				RandomCartoonNumberList.Remove(RandomCartoonNumberList.Last());
			}
		}

		private void AddCartoonToQueue(int count)
		{
			for(var i = 0; i < count; i++)
			{
				RandomCartoonNumberList.Add(rnd.Next(1, 101) % CheckedCartoons.Count);
			}
		}

		/// <summary>
		/// Метод начала просмотра серий
		/// </summary>
		private void StartWatch()
		{
			EpisodesCountRemainingString = "Осталось серий";

			CheckedEpisodes = ShuffleEpisode(CheckedEpisodes, GeneralSettings.RandomMixCount ?? 1);

			CurrentEpisodeIndex = 0;

			while(GeneralSettings.EpisodesCount > 0)
			{
				GeneralSettings.EpisodesCount--;
				NotifyOfPropertyChange(() => GeneralSettings);

				//цикл для переключения серии без потерь в количестве указанных просмотров
				do
				{
					IsSwitchEpisode = false;

					PlayEpisode(CheckedEpisodes[CurrentEpisodeIndex++]);

					if(IsSwitchEpisode)
					{
						if(GeneralSettings.EpisodesCount != GeneralSettings.AvailableEpisodesCount)
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

			//var webSite =
			//	CvDbContext.CartoonWebSites
			//			   .Include(csw => csw.ElementValues)
			//			   .First(cws => cws.Cartoons
			//								.Any(c => c.CartoonSeasons
			//										   .Any(cs => cs.CartoonSeasonId ==
			//													  episode.CartoonSeasonId)));

			var id = Cartoons
					 .First(c => c.CartoonSeasons
								  .Any(cs => cs.CartoonSeasonId ==
											 episode.CartoonSeasonId)).CartoonWebSites
					 .First().CartoonWebSiteId;

			var webSite = CvDbContext.CartoonWebSites
									 .Include(csw => csw.ElementValues)
									 .First(csw => csw.CartoonWebSiteId == id);

			var cartoon = Cartoons.First(c => c.CartoonSeasons
											   .Any(cs => cs.CartoonSeasonId ==
														  episode.CartoonSeasonId));

			var url = cartoon.CartoonUrls.First(cu => cu.CartoonId == cartoon.CartoonId &&
													  cu.CartoonWebSiteId == webSite.CartoonWebSiteId);

			//Попытки перейти на указанный url при нестабильном интернет соединении
			for(var i = 0; i < 10; i++)
			{
				try
				{
					Helper.Browser.Navigate()
						  .GoToUrl($"{url.Url}{episode.Number * 100 + episode.Number}/{episode.CartoonVoiceOver.UrlParameter}");
					break;
				}
				catch
				{
					Helper.Browser.Navigate().Refresh();
				}
			}

			CurrentDuration = episode.Duration;
			DelayedSkipDuration = episode.DelayedSkip;
			DelayedSkipCount = episode.SkipCount;
			CvDbContext.CartoonEpisodes.Find(episode.CartoonEpisodeId).LastDateViewed = DateTime.Now;
			CvDbContext.SaveChanges();

			//SetCartoonsSettings(_cartoon);
			StartVideoPlayer(webSite.ElementValues.First());

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

		#region Настройки мультсериалов

		/// <summary>
		/// Установка настроек мультсериалов
		/// </summary>
		/// <param name="cartoon">Мультсериал</param>
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
			if(episodeNum >= 101 && episodeNum <= 113)
			{
				Helper.Browser.Navigate().GoToUrl($"{url}?v=par");
			}

			if(episodeNum == 514 || episodeNum == 405)
			{
				Helper.CurrentSkipCount = 0;
			}
			else if(episodeNum >= 801 &&
					 episodeNum <= 814)
			{
				Helper.CurrentSkipCount = 8;
			}
			else if(episodeNum == 1901 ||
					 episodeNum == 1902 ||
					 episodeNum == 1905 ||
					 episodeNum == 1906 ||
					 (episodeNum >= 2001 && episodeNum <= 2010))
			{
				Helper.CurrentSkipCount = 10;
			}
			else if(episodeNum == 1903 ||
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
			else if(episodeNum == 1406 || episodeNum == 1909 ||
					 (episodeNum >= 2101 && episodeNum <= 2103))
			{
				Helper.CurrentSkipCount = 12;
			}
			else if((episodeNum >= 2106 && episodeNum <= 2110) ||
					 (episodeNum >= 2202 && episodeNum <= 2209) ||
					 episodeNum == 1802 || episodeNum == 1807 ||
					 episodeNum == 1809)
			{
				Helper.CurrentSkipCount = 13;
			}
			else if(episodeNum == 2201 ||
					 episodeNum == 1801 ||
					 episodeNum == 1808 ||
					 episodeNum == 1810)
			{
				Helper.CurrentSkipCount = 14;
			}
			else if(episodeNum == 2210)
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
		private void MonitoringAction(object stateInfo)
		{
			var autoEvent = (AutoResetEvent)stateInfo;

			NotifyOfPropertyChange(() => GeneralSettings);
			NotifyOfPropertyChange(() => EndDate);
			NotifyOfPropertyChange(() => EndTime);

			//NotifyEpisodesTime();

			//Таймер превысил длительность серии
			if(ElapsedTime > CurrentDuration || IsSwitchEpisode)
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
			if(DelayedSkipDuration > new TimeSpan() && Helper.Timer.Elapsed >= DelayedSkipDuration)
			{
				Helper.Timer.Stop();
				Helper.Msg.PressKey(Helper.VK_SPACE);
				Helper.Msg.PressKey(Helper.VK_RIGHT, DelayedSkipCount);
				Helper.Msg.PressKey(Helper.VK_SPACE);

				Helper.Timer.Start();
			}
		}

		public int DelayedSkipCount { get; set; }

		/// <summary>
		/// Мониторинг состояния паузы
		/// </summary>
		private void PauseMonitoring()
		{
			//Серия поставлена на паузу
			if(IsPause && Helper.Timer.IsRunning)
			{
				Helper.Timer.Stop();
				Helper.Msg.PressKey(Helper.VK_SPACE);
				((MainViewModel)Parent).WindowState = WindowState.Normal;
				return;
			}

			//Серия снята с паузы
			if(!IsPause && !Helper.Timer.IsRunning)
			{
				Helper.Timer.Start();
				((MainViewModel)Parent).WindowState = WindowState.Minimized;
				Helper.Msg.PressKey(Helper.VK_SPACE);
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

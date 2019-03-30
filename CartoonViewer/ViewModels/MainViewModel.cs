﻿namespace CartoonViewer.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models;
	using OpenQA.Selenium.Chrome;
	using static Helpers.Helper;
	using Timer = System.Threading.Timer;


	public class MainViewModel : Screen
	{
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();

		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
			}
		}




		private WindowState _windowState;
		public int SeriesCount { get; set; }

		public MainViewModel()
		{
			LoadCartoons();
		}

		

		private void LoadCartoons()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Load();
				Cartoons.AddRange(ctx.Cartoons.Local.OrderBy(c => c.CartoonId));
			}
		}

		public async void Start()
		{
			WindowState = WindowState.Minimized;

			await Task.Run(LaunchBrowser);
		}

		private Task LaunchBrowser()
		{
			StartBrowser();

			var autoEvent = new AutoResetEvent(false);

			var statusChecker = new StatusChecker(SeriesCount);

			var stateTimer = new Timer(statusChecker.CheckStatus,
									   autoEvent, new TimeSpan(0,0,0), new TimeSpan(0, 21, 30));

			autoEvent.WaitOne();
			autoEvent.Dispose();

			return Task.CompletedTask;
		}

		private void StartBrowser()
		{
			var options = new ChromeOptions();

			options.AddExtension($"{AppDomain.CurrentDomain.BaseDirectory}\\3.42.0_0.crx");

			Browser = new ChromeDriver(options);

			Thread.Sleep(10000);

			Browser.Navigate().GoToUrl("http://www.yandex.ru");

			//Для полной загрузки расширения Adblock
			//т.к. есть вероятность, что оно не загрузится
			Browser.Navigate().GoToUrl("http://sp.freehat.cc");

			Browser.Manage().Window.Maximize();
		}

		public void Closing()
		{
			Browser?.Quit();

		}

		/// <summary>
		/// Состояние окна
		/// </summary>
		public WindowState WindowState
		{
			get => _windowState;
			set
			{
				_windowState = value;
				NotifyOfPropertyChange(() => WindowState);
			}
		}
	}
}

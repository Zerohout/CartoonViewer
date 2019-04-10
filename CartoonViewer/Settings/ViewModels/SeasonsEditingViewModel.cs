namespace CartoonViewer.Settings.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public class SeasonsEditingViewModel : Screen, ISettingsViewModel
	{
		public SeasonsEditingViewModel(Season season)
		{
			if (season.Name == NewElementString) return;

			_season = CopySeason(season);
			_tempSeason = CopySeason(season);
		}

		public SeasonsEditingViewModel(List<Season> seasons)
		{
			Seasons.AddRange(seasons);
		}

		public SeasonsEditingViewModel()
		{
			_season = new Season();
			_tempSeason = new Season();
		}


		public Visibility AdvancedSettingsVisibility
		{
			get => Helper.AdvancedSettingsVisibility;
			set => Helper.AdvancedSettingsVisibility = value;
		}

		private BindableCollection<Season> _seasons = new BindableCollection<Season>();

		public BindableCollection<Season> Seasons
		{
			get => _seasons;
			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}


		private Season _tempSeason = new Season();

		public Season TempSeason
		{
			get => _tempSeason;
			set
			{
				_tempSeason = value;
				NotifyOfPropertyChange(() => TempSeason);
			}
		}


		private Season _season = new Season();

		public Season Season
		{
			get => _season;
			set
			{
				_season = value;
				NotifyOfPropertyChange(() => Season);
			}
		}

		public bool HasChanges { get; set; }

		public async void LoadDataAsync(int id)
		{
			Season result;
			using (var ctx = new CVDbContext())
			{
				await ctx.Seasons
				   .Where(s => s.SeasonId == id)
				   .Include(s => s.Episodes)
				   .LoadAsync();
				result = ctx.Seasons.Local.FirstOrDefault();
			}

			Season = CopySeason(result);
			TempSeason = CopySeason(result);
		}
	}
}

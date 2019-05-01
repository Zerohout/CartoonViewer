namespace CartoonViewer.MainMenu.ViewModels
{
	using System.Windows.Input;
	using Models.SettingModels;
	using static Helpers.Helper;
	using Keys = System.Windows.Forms.Keys;
	using Screen = Caliburn.Micro.Screen;

	public partial class MainMenuViewModel : Screen
	{
		public MainMenuViewModel()
		{

		}

		protected override void OnInitialize()
		{
			HotReg.RegisterGlobalHotkey(() => { IsPaused = !IsPaused; }, Keys.Pause, ModifierKeys.None);
			HotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
			HotReg.RegisterGlobalHotkey(() => { IsSwitchEpisode = true; }, Keys.Right, ModifierKeys.Control);
			//Helper.HotReg.RegisterGlobalHotkey(() => EpisodeCountString = (EpisodeCount - 1).ToString(), Keys.Delete, ModifierKeys.Shift);


			GeneralSettings = LoadGeneralSettings();
			GeneralSettings.EpisodesCount = GeneralSettings.DefaultEpisodesCount;
			LoadCartoons();
			base.OnInitialize();
		}

		public GeneralSettingsValue GeneralSettings { get; set; }


	}
}

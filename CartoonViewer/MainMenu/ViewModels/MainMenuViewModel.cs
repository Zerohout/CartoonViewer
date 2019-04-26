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
			HotReg.RegisterGlobalHotkey(() => { IsPause = !IsPause; }, Keys.Pause, ModifierKeys.None);
			HotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
			HotReg.RegisterGlobalHotkey(() => { IsSwitchEpisode = true; }, Keys.Right, ModifierKeys.Control);
			//Helper.HotReg.RegisterGlobalHotkey(() => EpisodeCountString = (EpisodeCount - 1).ToString(), Keys.Delete, ModifierKeys.Shift);

			

			LoadCartoons();
			GeneralSettings = LoadGeneralSettings();
			base.OnInitialize();
		}

		public GeneralSettingsValue GeneralSettings { get; set; }


	}
}

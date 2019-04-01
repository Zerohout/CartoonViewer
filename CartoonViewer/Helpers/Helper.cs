namespace CartoonViewer.Helpers
{
	using OpenQA.Selenium;

	public static class Helper
	{
		public const string RandomAddress = "rand.php";
		public const string VideoplayerPlayButton = "pjsdiv:nth-child(8) > pjsdiv > pjsdiv";
		public static string MainAddress = $"http://{CurrentCartoon}.freehat.cc/episode/";
		public static string CurrentCartoon = "";
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_CHAR = 0x102;
		public const int WM_SYSKEYDOWN = 0x104;
		public const int WM_SYSKEYUP = 0x105;
		public const int VK_LEFT = 0x25;
		public const int VK_RIGHT = 0x27;
		public const int VK_F = 0x46;
		public static int HWND = 0;

		public static IWebDriver Browser;
		public static MessageHelper Msg = new MessageHelper();
		public static bool FirstStart = true;
	}
}

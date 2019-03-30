

namespace CartoonViewer.Helpers
{
	using System.Threading;
	using System.Threading.Tasks;
	using OpenQA.Selenium;
	using WindowsInput;
	using WindowsInput.Native;
	using static Helper;

	public class StatusChecker
	{
		private int invokeCount;
		private readonly int maxCount;

		public StatusChecker(int count)
		{
			invokeCount = 0;
			maxCount = count;
		}

		public void CheckStatus(object stateInfo)
		{
			var autoEvent = (AutoResetEvent)stateInfo;
			PlayCartoon();


			if (invokeCount == maxCount)
			{
				invokeCount = 0;
				autoEvent.Set();
			}

			invokeCount++;
		}

		private void PlayCartoon()
		{
			Browser.Navigate().GoToUrl("https://sp.freehat.cc/episode/rand.php");
			var address = Browser.Url;

			if (ExtractNumber(address) < 200)
			{
				Browser.Navigate().GoToUrl($"{address}?v=par");
			}

			var sim = new InputSimulator();

			var el = Browser.FindElement(By.Id("videoplayer"));
			el.Click();


			if (FirstStart)
			{
				sim.Keyboard.Sleep(2000);
				sim.Keyboard.KeyPress(VirtualKeyCode.VK_F);
				FirstStart = false;
			}

			sim.Keyboard.Sleep(2000);

			for (var i = 0; i < 6; i++)
			{
				sim.Keyboard.KeyPress(VirtualKeyCode.RIGHT);
				sim.Keyboard.Sleep(500);
			}
		}

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

			return int.Parse(temp);
		}
	}
}

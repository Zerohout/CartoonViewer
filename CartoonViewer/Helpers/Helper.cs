using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonViewer.Helpers
{
	using OpenQA.Selenium;

	public static class Helper
	{
		public const string SouthParkAddress = "http://sp.freehat.cc/episode/rand.php";
		public const string FamilyGuyAddress = "http://grif.freehat.cc/episode/rand.php";
		public const string SimpsonsAddress = "http://simp.freehat.cc/episode/rand.php";
		public const string AmericanDadAddress = "http://dad.freehat.cc/episode/rand.php";
		public static IWebDriver Browser;
		public static bool FirstStart = true;
	}
}

namespace CartoonViewer.Migrations
{
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Creator;
	using static Helpers.Helper;

	internal sealed class Configuration : DbMigrationsConfiguration<CVDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(CVDbContext context)
		{
			//AddDataToDatabase(context);
		}


		private void AddDataToDatabase(CVDbContext context)
		{
			context.WebSites.Add(CreateWebSite(FreehatWebSite));
			context.Cartoons.AddRange(CreateCartoonList());
			context.VoiceOvers.AddRange(CreateVoiceOverList());
			context.SaveChanges();

			foreach (var cw in context.WebSites)
			{
				cw.ElementValues.Add(CreateElementValue());
			}

			context.SaveChanges();

			var website = context.WebSites.First();
			var cartoons = context.Cartoons.ToList();

			foreach (var cc in cartoons)
			{
				cc.WebSites.Add(website);
				cc.CartoonUrls.Add(CreateCartoonUrl(cc, website));
				context.Entry(cc).State = EntityState.Modified;
			}



			context.SaveChanges();
		}

		private CartoonUrl CreateCartoonUrl(Cartoon cartoon, WebSite webSite)
		{
			var url = "";
			switch (cartoon.Name)
			{
				case "����� ����":
					url = $"http://sp.freehat.cc/episode/";
					break;
				case "��������":
					url = $"http://grif.freehat.cc/episode/";
					break;
				case "��������":
					url = $"http://simp.freehat.cc/episode/";
					break;
				case "������������ ������":
					url = $"http://dad.freehat.cc/episode/";
					break;
			}

			return new CartoonUrl
			{
				Cartoon = cartoon,
				Url = url,
				Checked = true,
				WebSite = webSite,
				WebSiteUrl = webSite.Url
			};
		}
	}
}

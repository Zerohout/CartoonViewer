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
			context.CartoonWebSites.Add(CreateWebSite(FreehatWebSite));
			context.Cartoons.AddRange(CreateCartoonList());
			context.SaveChanges();


			foreach (var cw in context.CartoonWebSites)
			{
				cw.ElementValues.Add(CreateElementValue());
			}

			context.SaveChanges();

			var website = context.CartoonWebSites.First();
			var cartoons = context.Cartoons.ToList();

			foreach (var cc in cartoons)
			{
				if (cc.Name == "Южный парк")
				{
					cc.CartoonVoiceOvers.AddRange(CreateSouthParkVoiceOverList(cc.CartoonId));
				}

				cc.CartoonWebSites.Add(website);
				cc.CartoonUrls.Add(CreateCartoonUrl(cc, website));
				context.Entry(cc).State = EntityState.Modified;
			}

			context.SaveChanges();
		}

		private CartoonUrl CreateCartoonUrl(Cartoon cartoon, CartoonWebSite cartoonWebSite)
		{
			var url = "";
			switch (cartoon.Name)
			{
				case "Южный парк":
					url = $"http://sp.freehat.cc/episode/";
					break;
				case "Гриффины":
					url = $"http://grif.freehat.cc/episode/";
					break;
				case "Симпсоны":
					url = $"http://simp.freehat.cc/episode/";
					break;
				case "Американский папаша":
					url = $"http://dad.freehat.cc/episode/";
					break;
			}

			return new CartoonUrl
			{
				Cartoon = cartoon,
				Url = url,
				Checked = true,
				CartoonWebSite = cartoonWebSite,
				WebSiteUrl = cartoonWebSite.Url
			};
		}
	}
}

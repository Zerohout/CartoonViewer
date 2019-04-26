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
			if (context.CartoonWebSites.Any() is false)
			{
				context.CartoonWebSites.Add(new CartoonWebSite { Url = FreehatWebSite });
				context.SaveChanges();

				foreach(var cw in context.CartoonWebSites)
				{
					cw.ElementValues.Add(CreateElementValue());
				}

				context.SaveChanges();
			}


			//AddDataToDatabase(context);
		}




		private void AddDataToDatabase(CVDbContext context)
		{
			context.CartoonWebSites.Add(new CartoonWebSite { Url = FreehatWebSite });
			context.Cartoons.AddRange(CreateCartoonList());
			context.VoiceOvers.AddRange(CreateSouthParkVoiceOverList());
			context.SaveChanges();


			foreach(var cw in context.CartoonWebSites)
			{
				cw.ElementValues.Add(CreateElementValue());
			}

			context.SaveChanges();

			var website = context.CartoonWebSites.First();
			var cartoons = context.Cartoons.ToList();
			var voiceOvers = context.VoiceOvers.ToList();

			foreach(var cc in cartoons)
			{
				//if (cc.Name == "ёжный парк")
				//{
				//	cc.CartoonVoiceOvers.AddRange(voiceOvers);
				//}

				cc.CartoonWebSites.Add(website);
				cc.CartoonUrls.Add(CreateCartoonUrl(cc, website));
				context.Entry(cc).State = EntityState.Modified;
			}

			context.SaveChanges();

			var cart = cartoons.Find(c => c.Name == "ёжный парк");

			foreach(var vo in voiceOvers)
			{
				vo.Cartoons.Add(cart);
				context.Entry(vo).State = EntityState.Modified;
			}


			context.SaveChanges();
		}

		private CartoonUrl CreateCartoonUrl(Cartoon cartoon, CartoonWebSite cartoonWebSite)
		{
			var url = "";
			switch(cartoon.Name)
			{
				case "ёжный парк":
					url = $"http://sp.freehat.cc/episode/";
					break;
				case "√риффины":
					url = $"http://grif.freehat.cc/episode/";
					break;
				case "—импсоны":
					url = $"http://simp.freehat.cc/episode/";
					break;
				case "јмериканский папаша":
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

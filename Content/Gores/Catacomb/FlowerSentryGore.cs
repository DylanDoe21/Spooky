using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores.Catacomb
{
	public class FlowerSentryGore2 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			UpdateType = 910;
		}
	}
	
	public class FlowerSentryGore3 : FlowerSentryGore2
	{
	}

	public class FlowerSentryGore4 : FlowerSentryGore2
	{
	}

	public class FlowerSentryGore5 : FlowerSentryGore2
	{
	}

	public class FlowerSentryGore6 : FlowerSentryGore2
	{
	}

	public class FlowerSentryGore7 : FlowerSentryGore2
	{
	}
}
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores.CatacombLayer2
{
	public class LilyLeafGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			UpdateType = 910;
		}
	}

	public class LilyLeafGore2 : LilyLeafGore1
	{
	}

	public class LilyLeafGoreSmall1 : LilyLeafGore1
	{
	}

	public class LilyLeafGoreSmall2 : LilyLeafGore1
	{
	}
}
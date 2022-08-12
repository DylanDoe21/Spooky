using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores
{
	public class LeafGreen : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			gore.scale = Main.rand.NextFloat(0.4f, 0.6f);
			UpdateType = 910;
		}
	}

	public class LeafOrange : LeafGreen
	{
	}

	public class LeafRed : LeafGreen
	{
	}
}
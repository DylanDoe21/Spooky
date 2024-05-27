using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores.NoseCult
{
	public class NoseCultistMageCloth1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			UpdateType = 910;
		}
	}
	
	public class NoseCultistMageCloth2 : NoseCultistMageCloth1
	{
	}

	public class NoseCultistMageCloth3 : NoseCultistMageCloth1
	{
	}
}
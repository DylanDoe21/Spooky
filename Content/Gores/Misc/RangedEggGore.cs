using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.Misc
{
	public class RangedEggGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 2;
		}
	}

	public class RangedEggGore2 : RangedEggGore1
	{
	}
}
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.Misc
{
	public class GrowingBroccoliGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 10;
		}
	}

	public class GrowingBroccoliGore2 : GrowingBroccoliGore1
	{
	}

	public class GrowingBroccoliGore3 : GrowingBroccoliGore1
	{
	}

	public class GrowingBroccoliGore4 : GrowingBroccoliGore1
	{
	}
}
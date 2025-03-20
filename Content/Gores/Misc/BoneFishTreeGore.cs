using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.Misc
{
	public class BoneFishTreeGore : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 5;
		}
	}

	public class BoneFishTreeHangingBottomGore1 : BoneFishTreeGore
	{
	}

	public class BoneFishTreeHangingBottomGore2 : BoneFishTreeGore
	{
	}

	public class BoneFishTreeTopGore1 : BoneFishTreeGore
	{
	}

	public class BoneFishTreeTopGore2 : BoneFishTreeGore
	{
	}
}
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.Misc
{
	public class SentientSkullGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 5;
		}
	}

	public class SentientSkullGore2 : SentientSkullGore1
	{
	}
}
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.BigBone
{
	public class EyeFlowerStemGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 0;
		}
	}

	public class EyeFlowerStemGore2 : EyeFlowerStemGore1
	{
	}
}
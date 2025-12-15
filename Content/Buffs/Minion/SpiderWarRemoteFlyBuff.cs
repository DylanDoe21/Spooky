using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Buffs.Minion
{
	public class SpiderWarRemoteFlyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SpiderWarRemoteFly>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}

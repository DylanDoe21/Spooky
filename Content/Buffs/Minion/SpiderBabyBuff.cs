using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Buffs.Minion
{
	public class SpiderBabyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SpiderBabyGreen>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<SpiderBabyPurple>()] > 0 || 
			player.ownedProjectileCounts[ModContent.ProjectileType<SpiderBabyRed>()] > 0) 
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

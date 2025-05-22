using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Buffs.Minion
{
	public class GhostPepperMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier1>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier2>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier3>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier4>()] > 0) 
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

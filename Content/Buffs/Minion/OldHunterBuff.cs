using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Buffs.Minion
{
	public class OldHunterBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<OldHunterMelee>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<OldHunterRanged>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<OldHunterMage>()] > 0) 
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

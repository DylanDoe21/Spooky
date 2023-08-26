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
			//TODO: add the other old hunter minions here as they are added
			if (player.ownedProjectileCounts[ModContent.ProjectileType<OldHunterMelee>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<OldHunterRanged>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().OldHunter = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().OldHunter) 
			{
				player.buffTime[buffIndex] = 2;
			}
			else
			{
				player.buffTime[buffIndex] = 0;
			}
		}
	}
}

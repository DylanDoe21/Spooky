using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Minion
{
	public class EntityMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<EntityMinion1>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<EntityMinion2>()] > 0 || 
			player.ownedProjectileCounts[ModContent.ProjectileType<EntityMinion3>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().EntityMinion = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().EntityMinion) 
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

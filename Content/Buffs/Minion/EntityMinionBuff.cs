using Terraria;
using Terraria.ModLoader;

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

using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Minion
{
	public class RotGourdMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<RotGourdMinion>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().RotGourdMinion = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().RotGourdMinion) 
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

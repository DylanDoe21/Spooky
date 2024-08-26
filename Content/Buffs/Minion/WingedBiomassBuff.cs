using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Buffs.Minion
{
	public class WingedBiomassBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<WingedBiomass>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().WingedBiomass = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().WingedBiomass) 
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

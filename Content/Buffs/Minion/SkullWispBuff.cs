using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Minion
{
	public class SkullWispBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Skull Wisp");
			// Description.SetDefault("The skull wisp will fight with you");
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SkullWisp>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().SkullWisp = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().SkullWisp) 
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

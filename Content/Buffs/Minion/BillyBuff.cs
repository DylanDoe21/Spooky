using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Buffs.Minion
{
	public class BillyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Billy>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().Billy = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().Billy) 
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

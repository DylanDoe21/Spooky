using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Buffs.Minion
{
	public class TortumorMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<TortumorMinion>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().TumorMinion = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().TumorMinion) 
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

using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Buffs.Minion
{
	public class SoulSkullBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Skull");
			Description.SetDefault("The soul minions will fight with you");
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SoulSkullMinion>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<SkullTotem>()] > 0) 
			{
				player.GetModPlayer<SpookyPlayer>().SoulSkull = true;
			}

			if (player.GetModPlayer<SpookyPlayer>().SoulSkull) 
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

using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;
using Spooky.Core;

namespace Spooky.Content.Buffs.Pets
{
	public class GhostPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lil Ghost");
			Description.SetDefault("A spooky little ghost will guide you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().GhostPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<GhostPet>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 
				0f, 0f, ModContent.ProjectileType<GhostPet>(), 0, 0f, player.whoAmI);
			}

			if (player.GetModPlayer<SpookyPlayer>().GhostPet)
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

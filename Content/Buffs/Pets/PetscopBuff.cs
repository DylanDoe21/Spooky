using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class PetscopBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().PetscopPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<PetscopPet>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y,
				0f, 0f, ModContent.ProjectileType<PetscopPet>(), 0, 0f, player.whoAmI);
			}

			if (player.GetModPlayer<SpookyPlayer>().PetscopPet)
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

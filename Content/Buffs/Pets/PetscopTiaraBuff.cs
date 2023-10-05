using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class PetscopTiaraBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().PetscopTiaraPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<PetscopTiaraPet>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y,
				0f, 0f, ModContent.ProjectileType<PetscopTiaraPet>(), 0, 0f, player.whoAmI);
			}

			if (player.GetModPlayer<SpookyPlayer>().PetscopTiaraPet)
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

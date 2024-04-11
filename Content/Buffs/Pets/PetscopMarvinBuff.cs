using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class PetscopMarvinBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 2;
			
			player.GetModPlayer<SpookyPlayer>().PetscopMarvinPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<PetscopMarvinPet>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<PetscopMarvinPet>(), 0, 0f, player.whoAmI);
			}
		}
	}
}

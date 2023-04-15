using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class RotGourdPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().RotGourdPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<RotGourdPet>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y,
				0f, 0f, ModContent.ProjectileType<RotGourdPet>(), 0, 0f, player.whoAmI);
			}

			if (player.GetModPlayer<SpookyPlayer>().RotGourdPet)
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

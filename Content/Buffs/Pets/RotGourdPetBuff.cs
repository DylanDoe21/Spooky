using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class RotGourdPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rot-Gourd");
			Description.SetDefault("It still smells really bad");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().RotGourdPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<RotGourdPet>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y,
				0f, 0f, ModContent.ProjectileType<RotGourdPet>(), 0, 0f, player.whoAmI, 0f, 0f);
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

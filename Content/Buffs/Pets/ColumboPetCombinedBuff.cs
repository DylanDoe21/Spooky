using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class ColumboPetCombinedBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 2;
			
			player.GetModPlayer<SpookyPlayer>().ColumboPet = true;
			player.GetModPlayer<SpookyPlayer>().ColumbonePet = true;
			player.GetModPlayer<SpookyPlayer>().ColumbooPet = true;
			player.GetModPlayer<SpookyPlayer>().ColumborangePet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<Columbo>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Columbo>(), 0, 0f, player.whoAmI);
			}
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Columbone>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Columbone>(), 0, 0f, player.whoAmI);
			}
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Columboo>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Columboo>(), 0, 0f, player.whoAmI);
			}
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Columborange>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Columborange>(), 0, 0f, player.whoAmI);
			}
		}
	}
}

using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class PandoraBeanBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 2;
			
			player.GetModPlayer<SpookyPlayer>().PandoraBeanPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<PandoraBeanPet>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<PandoraBeanPet>(), 0, 0f, player.whoAmI);
			}
		}
	}
}

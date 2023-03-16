using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class SpookySpiritPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mini Spirit");
			Description.SetDefault("The mini spirit lingers around you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().SpookySpiritPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<SpookySpiritPet>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y, 
				0f, 0f, ModContent.ProjectileType<SpookySpiritPet>(), 0, 0f, player.whoAmI);
			}

			if (player.GetModPlayer<SpookyPlayer>().SpookySpiritPet)
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

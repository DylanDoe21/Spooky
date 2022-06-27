using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class SpookyWispBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spooky Wisp");
			Description.SetDefault("The spooky wisp is following you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<SpookyPlayer>().SpookyWispPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<SpookyWisp>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y,
				0f, 0f, ModContent.ProjectileType<SpookyWisp>(), 0, 0f, player.whoAmI, 0f, 0f);
			}

			if (player.GetModPlayer<SpookyPlayer>().SpookyWispPet)
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

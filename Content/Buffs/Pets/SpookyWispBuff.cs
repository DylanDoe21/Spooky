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
			player.buffTime[buffIndex] = 18000;
			player.GetModPlayer<SpookyPlayer>().SpookyWispPet = true;
			
			bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<SpookyWisp>()] <= 0;
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{	
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 
				ModContent.ProjectileType<SpookyWisp>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}

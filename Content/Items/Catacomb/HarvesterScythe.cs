using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class HarvesterScythe : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.crit = 5;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 64;          
			Item.height = 60;         
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.shoot = ModContent.ProjectileType<HarvesterScytheSlash>();
			Item.scale = 1.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int Slash = Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<HarvesterScytheSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
			Main.projectile[Slash].scale *= (Item.scale - 0.5f) * (player.meleeScaleGlove ? 1.1f : 1f);

            return false;
		}
	}
}

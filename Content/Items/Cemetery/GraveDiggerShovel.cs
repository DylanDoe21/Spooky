using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles.Cemetery;
 
namespace Spooky.Content.Items.Cemetery
{
	public class GraveDiggerShovel : ModItem
	{
		int numUses = -1;

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.crit = 10;
			Item.DamageType = DamageClass.Melee;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.channel = true;
			Item.width = 42;
			Item.height = 42;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 8;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<GraveDiggerShovelSwung>();
            Item.shootSpeed = 12f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;

			if (numUses > 1)
            {
                numUses = 0;
            }

			Projectile.NewProjectileDirect(source, position + (velocity * 20) + (velocity.RotatedBy(-1.57f * player.direction) * 20), Vector2.Zero, type, damage, knockback, player.whoAmI, numUses == 0 || numUses == 2 ? 0 : 1);
			
			return false;
		}
	}
}

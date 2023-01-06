using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class BoogerBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Booger Blaster");
			Tooltip.SetDefault("Charge up to shoot nasty boogers that slow enemies on hit"
			+ "\nThe longer you charge it, the stronger and faster the boogers will become"
			+ "\nConsumes bullets to use");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 64;          
			Item.height = 32;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 0f;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<BoogerBlasterProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
	}
}

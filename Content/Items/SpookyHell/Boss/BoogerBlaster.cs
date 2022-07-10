using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class BoogerBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Booger Blaster");
			Tooltip.SetDefault("Fires a spread of snot balls");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 25;           
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 56;           
			Item.height = 32;         
			Item.useTime = 10;         
			Item.useAnimation = 30;         
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item95;     
			Item.shoot = ModContent.ProjectileType<BlasterBooger>();
			Item.shootSpeed = 10f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-15, -5);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.direction == -1)
			{
				int currentShot = player.itemAnimation / Item.useTime - 1;

				Vector2 Speed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.PiOver4 / 3 * currentShot);
				Projectile.NewProjectile(source, position, Speed, type, damage, knockback, player.whoAmI);
			}
			else
			{
				int currentShot = player.itemAnimation / Item.useTime - 1;

				Vector2 Speed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.PiOver4 / 3 * -currentShot);
				Projectile.NewProjectile(source, position, Speed, type, damage, knockback, player.whoAmI);
			}

            return false;
		}
	}
}

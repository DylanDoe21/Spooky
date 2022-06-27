using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class PumpkinSlingshot : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seed Slingshot");
			Tooltip.SetDefault("Flings a spread of pumpkin seeds");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;           
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;       
			Item.width = 30;           
			Item.height = 34;         
			Item.useTime = 15;         
			Item.useAnimation = 30;         
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item5;     
			Item.shoot = ModContent.ProjectileType<SlingshotSeed>();
			Item.shootSpeed = 17f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
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

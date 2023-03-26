using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class EyeRocketLauncher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("I.C.U");
			Tooltip.SetDefault("Shoots homing eye missiles");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 100;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 78;
			Item.height = 28;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item98;
			Item.shoot = ModContent.ProjectileType<EyeRocket>();
			Item.shootSpeed = 10f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-34, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 50f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			Vector2 origVect = new Vector2(velocity.X, velocity.Y);
			Projectile.NewProjectile(source, position.X, position.Y, origVect.X, origVect.Y, ModContent.ProjectileType<EyeRocket>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 10)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class CeleryGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item64;
            Item.shoot = ModContent.ProjectileType<Celery>();
            Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 22f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(0, -3);
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, new Vector2(position.X, position.Y - 7), velocity, ModContent.ProjectileType<Celery>(), damage, knockback, player.whoAmI);

			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
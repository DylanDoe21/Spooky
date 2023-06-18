using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class GourdFlail : ModItem
    {
        public override void SetDefaults() 
        {
			Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 50;
            Item.height = 56;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Blue;
           	Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item10;
            Item.shoot = ModContent.ProjectileType<GourdFlailProj>();
            Item.shootSpeed = 12f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedByRandom((float)Math.PI / 16f), 
            ModContent.ProjectileType<GourdFlailProj>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 15)
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
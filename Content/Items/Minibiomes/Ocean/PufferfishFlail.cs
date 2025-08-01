using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class PufferfishFlail : ModItem
    {
        public override void SetDefaults() 
        {
			Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 42;
            Item.height = 38;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Blue;
           	Item.value = Item.buyPrice(gold: 1, silver: 50);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<PufferfishFlailProj>();
            Item.shootSpeed = 12f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedByRandom((float)Math.PI / 16f), 
            ModContent.ProjectileType<PufferfishFlailProj>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FishboneChunk>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
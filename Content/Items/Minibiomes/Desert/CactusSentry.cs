using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Projectiles.Minibiomes.Desert;
using Spooky.Content.Tiles.Minibiomes.Desert;
 
namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class CactusSentry : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.mana = 20;
			Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.width = 38;
            Item.height = 42;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item82;
            Item.shoot = ModContent.ProjectileType<CactusSentryProj>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int sentry = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(sentry))
            {
                Main.projectile[sentry].originalDamage = Item.damage;
            }
            
            player.UpdateMaxTurrets();

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TarPitCactusBlockItem>(), 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
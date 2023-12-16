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
    public class OldWoodStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = false; 
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;  
            Item.width = 52;
            Item.height = 58;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            Item.shoot = ModContent.ProjectileType<OldWoodStaffProj>();
            Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float num82 = (float)Main.mouseX + Main.screenPosition.X - position.X;
            float num83 = (float)Main.mouseY + Main.screenPosition.Y - position.Y;
            if (player.gravDir == -1f)
            {
                num83 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - position.Y;
            }
            float num84 = (float)Math.Sqrt((double)(num82 * num82 + num83 * num83));
            if ((float.IsNaN(num82) && float.IsNaN(num83)) || (num82 == 0f && num83 == 0f))
            {
                num82 = (float)player.direction;
                num83 = 0f;
                num84 = Item.shootSpeed;
            }
            else
            {
                num84 = Item.shootSpeed / num84;
            }
            num82 *= num84;
            num83 *= num84;
            float ai4 = Main.rand.NextFloat() * Item.shootSpeed * 0.75f * (float)player.direction;
            velocity = new Vector2(num82, num83);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai4, 0.0f);
            
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 25)
            .AddIngredient(ItemID.Amethyst, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
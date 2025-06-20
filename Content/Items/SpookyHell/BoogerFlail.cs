using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class BoogerFlail : ModItem
    {
		public override void SetDefaults() 
        {
			Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Orange;
           	Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<BoogerFlailProj>();
            Item.shootSpeed = 12f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedByRandom((float)Math.PI / 16f), 
            ModContent.ProjectileType<BoogerFlailProj>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:DemoniteBars", 15)
			.AddIngredient(ModContent.ItemType<SnotGlob>(), 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
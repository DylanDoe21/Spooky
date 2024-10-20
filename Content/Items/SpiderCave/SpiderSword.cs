using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SpiderSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.autoReuse = false;
            Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.width = 62;
            Item.height = 62;
            Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SpiderSwordProj>();
			Item.shootSpeed = 2.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpiderChitin>(), 15)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
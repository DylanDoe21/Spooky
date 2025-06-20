using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class PufferfishFlailMetal : ModItem
    {
        public override void SetDefaults() 
        {
			Item.damage = 110;
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
            Item.rare = ItemRarityID.LightRed;
           	Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<PufferfishFlailMetalProj>();
            Item.shootSpeed = 12f;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PufferfishFlail>(), 1)
            .AddIngredient(ModContent.ItemType<DunkleosteusHide>(), 12)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}
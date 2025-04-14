using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class CarrotWand : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15;
			Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true; 
            Item.width = 28;
            Item.height = 36;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item8;
			Item.shoot = ModContent.ProjectileType<CarrotWandOoze>();
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
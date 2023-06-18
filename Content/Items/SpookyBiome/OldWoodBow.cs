using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class OldWoodBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = false;         
            Item.width = 24;
            Item.height = 52;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;   
            Item.knockBack = 1;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(copper: 20);
            Item.UseSound = SoundID.Item5;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 7f;
			Item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
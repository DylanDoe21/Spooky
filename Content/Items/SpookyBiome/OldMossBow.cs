using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class OldMossBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = false;
            Item.width = 26;
            Item.height = 72;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;   
            Item.knockBack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item5;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 12f;
			Item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddIngredient(ModContent.ItemType<SpookyStoneItem>(), 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
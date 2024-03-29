using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class OldWoodSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = false;         
            Item.width = 36;
            Item.height = 44;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;   
            Item.knockBack = 2;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(copper: 20);
            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 7)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
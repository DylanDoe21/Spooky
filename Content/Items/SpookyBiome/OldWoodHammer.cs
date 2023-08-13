using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class OldWoodHammer : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 2;
            Item.hammer = 25;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = false;         
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Swing;   
            Item.knockBack = 2;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(copper: 20);
            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 8)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
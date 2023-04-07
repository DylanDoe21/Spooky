using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.MusicBox
{
    public class SpookyMenuBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyMenu"), 
            ModContent.ItemType<SpookyMenuBox>(), ModContent.TileType<SpookyMenuBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.hasVanityEffects = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.accessory = true;
            Item.width = 24;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
            Item.createTile = ModContent.TileType<SpookyMenuBoxTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyBiomeBox>())
            .AddIngredient(ModContent.ItemType<SpookyBiomeNightBox>())
            .AddIngredient(ModContent.ItemType<SpookyBiomeUGBox>())
            .AddIngredient(ModContent.ItemType<SpookyBiomeRainBox>())
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}
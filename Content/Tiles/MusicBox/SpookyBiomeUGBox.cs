using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.MusicBox
{
    public class SpookyBiomeUGBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Spooky Forest Underground)");
            Tooltip.SetDefault("'Spooky Spelunking' by Bananalizard");
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeUnderground"), 
            ModContent.ItemType<SpookyBiomeUGBox>(), ModContent.TileType<SpookyBiomeUGBoxTile>());
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.accessory = true;
            Item.width = 26;
            Item.height = 26;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = 1;
            Item.rare = 4;
            Item.value = Item.buyPrice(gold: 10);
            Item.createTile = ModContent.TileType<SpookyBiomeUGBoxTile>();
        }
    }
}
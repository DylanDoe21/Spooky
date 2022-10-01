using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.MusicBox;

namespace Spooky.Content.Items.Vinyl
{
    public class VinylAlley : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vinyl Disc (Alley)");
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Vinyl/Alley"), 
            ModContent.ItemType<VinylAlley>(), ModContent.TileType<VinylTileThing>());
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.accessory = true;
            Item.width = 32;
            Item.height = 22;
            Item.rare = ItemRarityID.Quest;
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}
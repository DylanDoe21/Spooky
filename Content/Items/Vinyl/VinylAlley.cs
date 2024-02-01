using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.MusicBox;

namespace Spooky.Content.Items.Vinyl
{
    public class VinylAlley : ModItem
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Vinyl/Alley"), 
            ModContent.ItemType<VinylAlley>(), ModContent.TileType<VinylTileThing>());
        }

        public override void SetDefaults()
        {
            Item.hasVanityEffects = true;
            Item.accessory = true;
            Item.width = 32;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}
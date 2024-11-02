using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.MusicBox;

namespace Spooky.Content.Items.Vinyl
{
    [LegacyName("RaveyardDisc3")]
    public class RaveyardDisc2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/RaveyardLegacy"), 
            ModContent.ItemType<RaveyardDisc2>(), ModContent.TileType<VinylTileThing7>());
        }

        public override void SetDefaults()
        {
            Item.hasVanityEffects = true;
            Item.accessory = true;
            Item.width = 32;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 25);
        }
    }
}
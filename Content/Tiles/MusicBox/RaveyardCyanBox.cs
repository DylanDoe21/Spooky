using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.MusicBox
{
    public class RaveyardCyanBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/RaveyardLegacy2"), 
            ModContent.ItemType<RaveyardCyanBox>(), ModContent.TileType<RaveyardCyanBoxTile>());
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
			Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
            Item.createTile = ModContent.TileType<RaveyardCyanBoxTile>();
        }
    }
}
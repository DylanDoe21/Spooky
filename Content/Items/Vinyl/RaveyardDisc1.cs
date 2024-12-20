using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.MusicBox;

namespace Spooky.Content.Items.Vinyl
{
    public class RaveyardDisc1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Raveyard1"), 
            ModContent.ItemType<RaveyardDisc1>(), ModContent.TileType<VinylTileThing6>());
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

    public class RaveyardDisc2 : RaveyardDisc1
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Raveyard2"), 
            ModContent.ItemType<RaveyardDisc2>(), ModContent.TileType<VinylTileThing7>());
        }
    }

    public class RaveyardDisc3 : RaveyardDisc1
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/RaveyardLegacy"), 
            ModContent.ItemType<RaveyardDisc3>(), ModContent.TileType<VinylTileThing8>());
        }
    }

    public class RaveyardDisc4 : RaveyardDisc1
    {
        public override void SetStaticDefaults()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/RaveyardLegacy2"), 
            ModContent.ItemType<RaveyardDisc4>(), ModContent.TileType<VinylTileThing9>());
        }
    }
}
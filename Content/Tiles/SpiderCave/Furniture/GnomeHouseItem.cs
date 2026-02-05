using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
    public class GnomeHouse1Item : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GnomeHouse1>());
            Item.width = 16;
			Item.height = 16;
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class GnomeHouse2Item : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GnomeHouse2>());
            Item.width = 16;
			Item.height = 16;
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class GnomeHouse3Item : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GnomeHouse3>());
            Item.width = 16;
			Item.height = 16;
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class GnomeHouse4Item : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GnomeHouse4>());
            Item.width = 16;
			Item.height = 16;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
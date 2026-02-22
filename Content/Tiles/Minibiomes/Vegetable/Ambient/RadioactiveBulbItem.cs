using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable.Ambient
{
    public class RadioactiveBulbItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadioactiveBulb>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class RadioactiveBulbSmallItem : ModItem
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Vegetable/Ambient/RadioactiveBulbSmall";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadioactiveBulbSmall>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}
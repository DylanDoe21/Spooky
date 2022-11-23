using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.SpookyHell.Tree
{
    public class EyeSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye Stalk Seed");
            Tooltip.SetDefault("Plants eye stalk saplings");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 18;
			Item.height = 26;
			Item.useTime = 7;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 999;
			Item.createTile = ModContent.TileType<EyeSapling>();
        }
    }
}
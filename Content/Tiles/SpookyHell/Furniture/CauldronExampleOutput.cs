using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class CauldronExampleOutput : ModItem, ICauldronOutput
    {
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Example cauldron output");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<EyeBed>();
		}
    }
}
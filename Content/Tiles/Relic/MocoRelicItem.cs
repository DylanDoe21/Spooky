using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Relic
{
	public class MocoRelicItem : ModItem
	{
		public override void SetStaticDefaults() 
        {
			DisplayName.SetDefault("Moco Relic");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
        {
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 30;
			Item.height = 44;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 99;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.createTile = ModContent.TileType<MocoRelic>();
		}
	}
}
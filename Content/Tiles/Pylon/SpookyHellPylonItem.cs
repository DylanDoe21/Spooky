using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Pylon
{
	public class SpookyHellPylonItem : ModItem
    {
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Valley of Eyes Pylon");
			Tooltip.SetDefault("Teleport to another pylon, regardless of how many villagers are nearby"
			+ "\nYou can only place one per type and in the matching biome");
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
			Item.maxStack = 999;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);  
			Item.createTile = ModContent.TileType<SpookyHellPylon>();
		}
	}
}
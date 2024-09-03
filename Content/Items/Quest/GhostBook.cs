using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Quest
{
	public class GhostBookBlue : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }

		public override bool? UseItem(Player player)
		{
			Item.ChangeItemType(ModContent.ItemType<GhostBookGreen>());

			return true;
		}
	}

    public class GhostBookGreen : GhostBookBlue
	{
		public override bool? UseItem(Player player)
		{
			Item.ChangeItemType(ModContent.ItemType<GhostBookRed>());

			return true;
		}
	}

    public class GhostBookRed : GhostBookBlue
	{
		public override bool? UseItem(Player player)
		{
			Item.ChangeItemType(ModContent.ItemType<GhostBookBlue>());

			return true;
		}
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class MinePressureSensor : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.rare = ItemRarityID.Quest;  
            Item.value = Item.buyPrice(copper: 50);
            Item.maxStack = 9999;
        }
	}
}

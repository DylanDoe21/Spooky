using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Quest
{
	[AutoloadEquip(EquipType.Front)]
	public class StitchedCloak : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().StitchedCloak = true;
		}
	}
}

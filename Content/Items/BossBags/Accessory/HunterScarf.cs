using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [AutoloadEquip(EquipType.Neck)]
	public class HunterScarf : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 18);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().HunterScarf = true;
        }
	}
}

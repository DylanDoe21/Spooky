using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class GooChompers : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PeptoStomach>();
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SpookyPlayer>().GooChompers = true;
		}
	}
}
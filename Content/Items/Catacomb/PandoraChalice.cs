using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraChalice : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraCross>();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().PandoraChalice = true;
        }
    }
}
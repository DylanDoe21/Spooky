using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraCuffs : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraRosary>();
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraRosary : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraChalice>();
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().PandoraRosary = true;
        }
    }
}
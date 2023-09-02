using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraCross : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraCuffs>();
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (!player.HasBuff(ModContent.BuffType<PandoraCrossCooldown>()))
            {
                player.GetModPlayer<SpookyPlayer>().PandoraCross = true;
            }
        }
    }
}
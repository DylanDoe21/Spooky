using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class MonumentMythosPyramid : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;  
            Item.value = Item.buyPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.HasBuff(ModContent.BuffType<MonumentMythosCooldown>()))
            {
                player.GetModPlayer<SpookyPlayer>().MonumentMythosPyramid = true;
                player.statDefense += 40;
            }
        }
    }
}
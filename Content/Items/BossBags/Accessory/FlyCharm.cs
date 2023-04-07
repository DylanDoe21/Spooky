using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [LegacyName("PumpkinCore")]
    public class FlyCharm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (!player.HasBuff(ModContent.BuffType<FlyCooldown>()))
            {
                player.GetModPlayer<SpookyPlayer>().FlyAmulet = true;
            }
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Catacomb
{
    public class CrossCharm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 15);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (!player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                player.endurance += 0.35f;
                player.GetModPlayer<SpookyPlayer>().CrossCharmShield = true;
            }
        }
    }
}
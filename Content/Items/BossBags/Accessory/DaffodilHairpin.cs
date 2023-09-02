using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [AutoloadEquip(EquipType.Face)]
    public class DaffodilHairpin : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (!player.HasBuff(ModContent.BuffType<DaffodilHairpinCooldown>()))
            {
                player.GetModPlayer<SpookyPlayer>().DaffodilHairpin = true;
                player.thorns += 15f;
            }
        }
    }
}
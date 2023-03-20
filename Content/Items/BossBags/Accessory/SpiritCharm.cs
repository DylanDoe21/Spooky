using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class SpiritCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Charm");
            Tooltip.SetDefault("When hit, you will sometimes release homing spirit particles around you"
            + "\n15% increased movement speed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

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
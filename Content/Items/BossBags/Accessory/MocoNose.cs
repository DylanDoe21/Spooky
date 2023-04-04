using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class MocoNose : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Snotty Schnoz");
            /* Tooltip.SetDefault("Hitting enemies will sometimes drop globs of snot you can pick up"
            + "\nAfter picking up fifteen snot globs, using weapons will shoot snot for a short period"
            + "\nEnemies will stop dropping snot for thirty seconds after this effect is triggered"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 12);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().MocoNose = true;
        }
    }
}
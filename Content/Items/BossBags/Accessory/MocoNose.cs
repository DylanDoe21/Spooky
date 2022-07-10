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
            DisplayName.SetDefault("Snotty Schnoz");
            Tooltip.SetDefault("Getting hit will sometimes release homing boogers around you"
            + "\nThe damage of the boogers will scale based on how much damage you took");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().MocoNose = true;
        }
    }
}
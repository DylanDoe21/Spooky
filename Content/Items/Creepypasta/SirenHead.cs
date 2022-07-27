using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.Creepypasta
{
    [AutoloadEquip(EquipType.Beard)]
    public class SirenHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Siren Head");
            Tooltip.SetDefault("Wee-woo");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(gold: 10);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().SirenHead = true;
        }
    }
}
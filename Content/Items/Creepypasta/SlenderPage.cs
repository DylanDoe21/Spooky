using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Creepypasta
{
    public class SlenderPage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The 8th Page");
            Tooltip.SetDefault("Press the special ability hotkey to enter a slender frenzy"
            + "\nDuring this frenzy, you will sometimes spawn shadow tentacles and enemies will drop pages"
            + "\nPicking up pages will temporarily increase the frequency, damage, and range of the tentacles");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 42;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 3);
        }
       
        /*
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().MocoNose = true;
        }
        */
    }
}
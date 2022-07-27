using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Creepypasta
{
    public class VHS : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("VHS Tape");
            Tooltip.SetDefault("Summons the real gabriel to watch over you"
            + "\nEvery 20 seconds, gabriel will spawn a temporary shadow entity to fight with you");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
        }
       
        /*
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().MocoNose = true;
        }
        */
    }
}
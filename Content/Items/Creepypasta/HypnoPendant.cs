using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Creepypasta
{
    public class HypnoPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hypnotic Pendant");
            Tooltip.SetDefault("When hit, you spawn a temporary floating hypnotic pendant with an aura around it"
            + "\nEnemies in the aura are damaged, and non boss enemies are inflicted with confusion"
            + "\nThe pendant will last for 10 seconds, and this ability cannot activate again for 30 seconds");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
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
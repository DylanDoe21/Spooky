using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class SentientFleshWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Eye Lasher");
            Tooltip.SetDefault("Lashes out two whips at once"
            + "\nHitting enemies will cause them to take 10% more damage from summons"
            + "\nCritical hits will permanently lower enemy defense, caps after 10 defense is lost");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 50;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class SpookyHellKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Valley Key");
            /* Tooltip.SetDefault("Unlocks an eye chest in the catacombs"
            + "\nCannot be used until plantera has been defeated"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class ChestFood : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chest Monster Food");
            /* Tooltip.SetDefault("Perhaps feeding it to a chest monster may awaken it"
            + "\nOpens one monster chest in the eye valley"
            + "\n'A chest monster's favorite delicacy'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);
            Item.maxStack = 99;

        }
    }
}
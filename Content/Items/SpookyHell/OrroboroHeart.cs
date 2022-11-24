using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class OrroboroHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Heart");
            Tooltip.SetDefault("Can be used at the alchemical cauldron\nWhen combined with certain weapons, it will bring them sentient powers");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 44;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
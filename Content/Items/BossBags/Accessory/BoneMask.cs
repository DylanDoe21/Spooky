using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class BoneMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Disciple's Mask");
            Tooltip.SetDefault("Fire off homing skull wisps while you are flying or running"
            + "\nThe rate the wisps are fired scales with your current speed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            //player.GetModPlayer<SpookyPlayer>().BoneMask = true;
        }
    }
}
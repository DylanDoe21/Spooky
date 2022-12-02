using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.Catacomb
{
    public class CrossCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cross Charm");
            Tooltip.SetDefault("Grants you a magic cross that reduces damage the next time you are hit"
            + "\nAfter being hit, the the cross will disappear and regenerate after 10 seconds");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().CrossCharmShield = true;
        }
    }
}
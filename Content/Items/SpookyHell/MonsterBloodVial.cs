using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class MonsterBloodVial : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Monster Blood");
            /* Tooltip.SetDefault("Massively increases life regeneration while not moving"
            + "\nWhile moving, you gain 5% increased critical strike chance"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.velocity.X == 0 && player.velocity.Y == 0)
            {
                player.lifeRegen += 15;
            }
            else
            {
                player.GetCritChance<GenericDamageClass>() += 5;
            }   
        }
    }
}
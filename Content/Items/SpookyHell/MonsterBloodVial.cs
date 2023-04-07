using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell
{
    public class MonsterBloodVial : ModItem
    {
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
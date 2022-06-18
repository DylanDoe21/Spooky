using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class OrroboroHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of Orro-Boro");
            Tooltip.SetDefault("Increases damage by 15% and critical strike chance by 10% when below half health"
            + "\nDisables any life regeneration while above half health"
            + "\nEnemies are far more likely to target you");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = 5;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < (player.statLifeMax2 / 2))
            {
                player.GetDamage<GenericDamageClass>() += 0.25f;
                player.GetCritChance<GenericDamageClass>() += 15;
            }

            if (player.statLife >= (player.statLifeMax2 / 2))
            {
                if (player.lifeRegen >= 0) 
                {
                    player.lifeRegen = 0;
                }
            }
        }
    }
}
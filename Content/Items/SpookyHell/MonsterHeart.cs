using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class MonsterHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monster Heart");
            Tooltip.SetDefault("Increases damage by 10% and critical strike chance by 8% when below half health"
            + "\nHalves life regeneration while above half health"
            + "\nEnemies are far more likely to target you");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < (player.statLifeMax2 / 2))
            {
                player.GetDamage<GenericDamageClass>() += 0.10f;
                player.GetCritChance<GenericDamageClass>() += 8;
            }

            if (player.statLife >= (player.statLifeMax2 / 2))
            {
                if (player.lifeRegen > 0) 
                {
                    player.lifeRegen = player.lifeRegen / 2;
                }
            }
        }
    }
}
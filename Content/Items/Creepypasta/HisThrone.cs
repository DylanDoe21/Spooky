using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Creepypasta
{
    public class HisThrone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("His Throne");
            Tooltip.SetDefault("Summons a strange moon that has different abilities at day and night"
            + "\nDuring the day, the moon will orbit you, damaging hit enemies and blocking projectiles"
            + "\nAt night, the moon will shoot radio waves, dealing low damage but inflicting tons of debuffs");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 8);
        }
       
        /*
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().MocoNose = true;
        }
        */
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class PumpkinCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core of the Pumpkin");
            Tooltip.SetDefault("Summons a swarm of damaging flies around you"
            + "\nFlies that hit enemies will damage them and then disappear"
            + "\nOnce all flies have disappeared, the fly swarm will respawn");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = 2;
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().PumpkinCore = true;
        }
    }
}
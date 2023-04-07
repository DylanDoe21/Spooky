using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class OrroboroEmbryo : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 25);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<SpookyPlayer>().OrroboroEmbyro = true;
        }
    }
}
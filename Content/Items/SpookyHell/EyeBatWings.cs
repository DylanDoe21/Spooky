using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Diagnostics;
using System;

namespace Spooky.Content.Items.SpookyHell
{
    [AutoloadEquip(EquipType.Wings)]
    public class EyeBatWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bat n' Eye Wings");
            Tooltip.SetDefault("Gives infinite flight time and very fast flight speed"); 
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 12);
        }
		
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wingTimeMax = 85;
        }

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
		ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) 
		{
			ascentWhenFalling = 0.85f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1.1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.095f;
		}
		
		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 10f;
			acceleration *= 5.5f;
		}
    }
}
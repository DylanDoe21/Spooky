using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Items.Catacomb.Misc
{
	public class Slot4Unlocker : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 46;
            Item.consumable = true;
			Item.noUseGraphic = true;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.HoldUp;
			Item.rare = ItemRarityID.Yellow;
            Item.maxStack = 1;
		}

		public override bool? UseItem(Player player)
        {
			player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4 = true;

			return true;
        }
	}
}
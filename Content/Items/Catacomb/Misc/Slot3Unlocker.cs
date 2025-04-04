using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Items.Catacomb.Misc
{
	public class Slot3Unlocker : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 48;
            Item.consumable = true;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Pink;
            Item.maxStack = 1;
		}

		public override bool? UseItem(Player player)
        {
			SoundEngine.PlaySound(SoundID.Item64, player.Center);
			SoundEngine.PlaySound(SoundID.DoubleJump with { Volume = 2f }, player.Center);

			player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3 = true;

			return true;
        }
	}
}
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
			Item.width = 34;
			Item.height = 46;
            Item.consumable = true;
			Item.noUseGraphic = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
			Item.rare = ItemRarityID.Pink;
            Item.maxStack = 1;
		}

		public override bool? UseItem(Player player)
        {
			SoundEngine.PlaySound(SoundID.ResearchComplete with { Pitch = 1.5f }, player.Center);
			SoundEngine.PlaySound(SoundID.DoubleJump with { Volume = 2f }, player.Center);

			player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3 = true;

			for (int numGores = 1; numGores <= 30; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(player.GetSource_ItemUse(player.HeldItem), player.Center, new Vector2(Main.rand.Next(-22, 22), Main.rand.Next(-15, -2)), Main.rand.Next(276, 283));
                }
            }

			return true;
        }
	}
}
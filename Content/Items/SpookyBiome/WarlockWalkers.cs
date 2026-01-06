using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyBiome
{
    //[AutoloadEquip(EquipType.Shoes)]
	public class WarlockWalkers : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 40;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.accRunSpeed = 5f;

			if (!hideVisual && !player.sleeping.isSleeping && player.velocity.Y == 0 && player.velocity.X != 0)
			{
				for (int k = 0; k < 1; k++)
				{
					int walkDust = Dust.NewDust(new Vector2(player.position.X - 2f, player.position.Y + (float)player.height - 2f), player.width + 4, 4, DustID.AmberBolt, 0f, 0f, 100, default, 1f);
					Main.dust[walkDust].velocity = -player.velocity;
					Main.dust[walkDust].noGravity = true;
				}
			}
		}
	}
}
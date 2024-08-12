using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyHell
{
	[AutoloadEquip(EquipType.Wings)]
	public class SnotWings : ModItem
	{
		public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound) { Pitch = 0.45f, Volume = 0.3f };

		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(50, default, 1.025f);
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noFallDmg = true;
        }

		public override bool WingUpdate(Player player, bool inUse)
		{
			//only use the custom animation if you are flying and your wing time hasnt run out
			if (inUse && player.wingTime > 0)
			{
				player.wingFrameCounter++;

				if (player.wingFrameCounter >= 4)
				{
					SoundEngine.PlaySound(FlyingSound, player.Center);

					player.wingFrameCounter = 0;
				}

				player.wingFrame = 1 + player.wingFrameCounter / 2;

				//spawn ooze gores as the player is flying
                if (Main.rand.NextBool(10))
                {
					int newGore = Gore.NewGore(player.GetSource_FromThis(), player.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(0, 12)), Vector2.Zero, Main.rand.Next(1024, 1027));
					//TODO: can dye shaders be applied to gores?
                }

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
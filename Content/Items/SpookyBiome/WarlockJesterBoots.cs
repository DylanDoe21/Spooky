using Microsoft.Xna.Framework;
using Spooky.Content.Items.Minibiomes.Christmas;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome
{
    //[AutoloadEquip(EquipType.Shoes)]
	public class WarlockJesterBoots : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 40;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.autoJump = true;
			player.accRunSpeed = 6.5f;
			player.maxRunSpeed = 6.5f;
			player.GetJumpState<WarlockJesterBootJump1>().Enable();
			player.GetJumpState<WarlockJesterBootJump2>().Enable();
			player.GetJumpState<WarlockJesterBootJump3>().Enable();

			if (!hideVisual && !player.sleeping.isSleeping && player.velocity.Y == 0 && player.velocity.X != 0)
			{
				for (int k = 0; k < 1; k++)
				{
					int walkDust = Dust.NewDust(new Vector2(player.position.X - 2f, player.position.Y + (float)player.height - 2f), player.width + 4, 4, DustID.AmberBolt, 0f, 0f, 100, default, 1f);
					Main.dust[walkDust].noGravity = true;
					Main.dust[walkDust].velocity = -player.velocity;
				}
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<WarlockWalkers>(), 1)
            .AddIngredient(ModContent.ItemType<KrampusJumpShoe>(), 1)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
	}

	public class WarlockJesterBootJump1 : ExtraJump
	{
        public static readonly SoundStyle JumpSound = new("Spooky/Content/Sounds/WarlockBootJump", SoundType.Sound) { Volume = 0.3f };

		public override Position GetDefaultPosition() => new Before(CloudInABottle);

		public override float GetDurationMultiplier(Player player) => 1f;

		public override void OnStarted(Player player, ref bool playSound)
		{
			playSound = false;

            SoundEngine.PlaySound(JumpSound, player.Center);

			Vector2 playerVelocity = player.velocity * 0.4f + Vector2.UnitY;
            Vector2 offset = player.Bottom;

            for (int i = 0; i <= 10; i++)
            {
                Vector2 position = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 10) * new Vector2(1f, 0.25f);
                Vector2 velocity = -playerVelocity - position * 1.25f;
                position = position * Main.rand.Next(35, 56) + offset;
                Dust dust = Dust.NewDustPerfect(position, DustID.WitherLightning, velocity);
                dust.noGravity = true;
            }
		}

		public override void ShowVisuals(Player player)
		{
			if (Main.rand.NextBool())
			{
				int offsetY = (player.gravDir == -1f) ? -6 : player.height;

				Dust dust = Dust.NewDustDirect(player.position + new Vector2(-4, offsetY), player.width + 8, 4, DustID.AmberBolt, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, default, 1.5f);
				dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
				dust.noGravity = true;
			}
		}
	}

	public class WarlockJesterBootJump2 : WarlockJesterBootJump1
	{
		public override Position GetDefaultPosition() => new Before(CloudInABottle);

		public override float GetDurationMultiplier(Player player) => 1.2f;
	}

	public class WarlockJesterBootJump3 : WarlockJesterBootJump1
	{
		public override Position GetDefaultPosition() => new Before(CloudInABottle);

		public override float GetDurationMultiplier(Player player) => 1.4f;
	}
}
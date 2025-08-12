using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    //[AutoloadEquip(EquipType.Shoes)]
	public class KrampusJumpShoe : ModItem
	{
		public bool allowJump = true;

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
			//player.GetModPlayer<SpookyPlayer>().KrampusJumpShoe = true;
			player.GetJumpState<KrampusShoeJump1>().Enable();
			player.GetJumpState<KrampusShoeJump2>().Enable();
			player.GetJumpState<KrampusShoeJump3>().Enable();
		}
	}

	public class KrampusShoeJump1 : ExtraJump
	{
        public static readonly SoundStyle JumpSound = new("Spooky/Content/Sounds/KrampusShoeJump", SoundType.Sound) { Volume = 0.45f };

		public override Position GetDefaultPosition() => new Before(CloudInABottle);

		public override float GetDurationMultiplier(Player player) => 1.5f;

		public override void OnStarted(Player player, ref bool playSound)
		{
			playSound = false;

            SoundEngine.PlaySound(JumpSound, player.Center);

			Vector2 playerVelocity = player.velocity * 0.4f + Vector2.UnitY;
            Vector2 offset = player.Bottom;

            for (int i = 0; i <= 20; i++)
            {
                Vector2 position = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 20) * new Vector2(1f, 0.25f);
                Vector2 velocity = -playerVelocity - position * 1.25f;
                position = position * Main.rand.Next(1, 35) + offset;
                Dust dust = Dust.NewDustPerfect(position, DustID.SnowflakeIce, velocity);
                dust.noGravity = true;
            }
		}

		public override void ShowVisuals(Player player)
		{
			int offsetY = (player.gravDir == -1f) ? -6 : player.height;

			Dust dust = Dust.NewDustDirect(player.position + new Vector2(-4, offsetY), player.width + 8, 4, ModContent.DustType<CartoonStar>(), -player.velocity.X * .5f, player.velocity.Y * 0.5f, 100, default, 0.5f);
			dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
		}
	}

	public class KrampusShoeJump2 : KrampusShoeJump1
	{
		public override Position GetDefaultPosition() => new Before(CloudInABottle);
	}

	public class KrampusShoeJump3 : KrampusShoeJump1
	{
		public override Position GetDefaultPosition() => new Before(CloudInABottle);
	}
}
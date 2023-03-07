using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagBoro : ModItem
	{
		public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Treasure Bag (Boro)");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
        {
			Item.width = 32;
			Item.height = 32;
			Item.consumable = true;
			Item.expert = true;
			Item.rare = ItemRarityID.Expert;
			Item.maxStack = 999;
		}

		public override bool CanRightClick() 
		{
			return true;
		}

		public override void OpenBossBag(Player player)
        {
			player.TryGettingDevArmor(player.GetSource_OpenItem(Type));

			int[] MainItem1 = new int[] { ModContent.ItemType<EyeFlail>(), ModContent.ItemType<Scycler>(), ModContent.ItemType<EyeRocketLauncher>() };
			int[] MainItem2 = new int[] { ModContent.ItemType<MouthFlamethrower>(), ModContent.ItemType<LeechStaff>(), ModContent.ItemType<LeechWhip>() };

            player.QuickSpawnItem(player.GetSource_OpenItem(Type), Main.rand.Next(MainItem1));
			player.QuickSpawnItem(player.GetSource_OpenItem(Type), Main.rand.Next(MainItem2));

			player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<OrroboroChunk>(), Main.rand.Next(20, 35));
				
			//expert item
			player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<OrroboroEmbryo>());
		}

		public override int BossBagNPC => ModContent.NPCType<BoroHead>();

		public override Color? GetAlpha(Color lightColor) 
		{
			// Makes sure the dropped bag is always visible
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}

		public override void PostUpdate() 
		{
			// Spawn some light and dust when dropped in the world
			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

			if (Item.timeSinceItemSpawned % 12 == 0) 
			{
				Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

				// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
				Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
				float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
				Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

				Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
				dust.scale = 0.5f;
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
				dust.noLight = true;
				dust.alpha = 0;
			}
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) 
		{
			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			Rectangle frame;

			if (Main.itemAnimations[Item.type] != null) 
			{
				// In case this item is animated, this picks the correct frame
				frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
			}
			else 
			{
				frame = texture.Frame();
			}

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
			Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f) 
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(113, 38, 201, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;

				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(113, 38, 201, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}
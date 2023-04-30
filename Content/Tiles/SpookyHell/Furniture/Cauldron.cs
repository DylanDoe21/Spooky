using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Sentient;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class Cauldron : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(0, 128, 0), name);
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
			{
				var existing = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<CauldronDummy>()).FirstOrDefault();
				if (existing == default)
				{
					Projectile.NewProjectile(new EntitySource_TileUpdate(i, j), new Vector2(i + 2, j + 2) * 16, Vector2.Zero, ModContent.ProjectileType<CauldronDummy>(), 0, 0);
				}
			}
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}
    }

	public class CauldronDummy : ModProjectile
	{
		public int shakeTimer = 0;

		Vector2 scaleVec;
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
			Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
			if (shakeTimer-- > 0)
			{
				float sin = shakeTimer * 0.08971428571f * 2;
				scaleVec = new Vector2(MathF.Sin(sin), -MathF.Sin(sin));
			}
			else
			{
				scaleVec = Vector2.Zero;
			}

			Projectile.frameCounter++;
			if (Projectile.frameCounter % 6 == 0)
			{
				Projectile.frame++;
			}
			
			Projectile.frame %= Main.projFrames[Projectile.type];
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
			Texture2D bubbleTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Furniture/CauldronDummyBubbles").Value;
			int frameHeight = tex.Height / Main.projFrames[Projectile.type];
			Rectangle frameBox = new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);

			Main.spriteBatch.Draw(tex, Projectile.Bottom - Main.screenPosition, frameBox, lightColor, Projectile.rotation, new Vector2(tex.Width / 2, frameHeight), Projectile.scale * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);

			if (shakeTimer <= 0)
			{
				Main.spriteBatch.Draw(bubbleTex, Projectile.Bottom - Main.screenPosition, frameBox, lightColor, Projectile.rotation, new Vector2(tex.Width / 2, frameHeight), Projectile.scale * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
			}

            return false;
        }
    }

    public class CauldronSystem : ModSystem
	{
		public static Dictionary<int, int> transform = new Dictionary<int, int>();

		public static List<int> inputItems = new List<int>();
        public static List<int> outputItems = new List<int>();

        public override void Load()
        {
			//melee
			AddTransformation(ItemID.Chik, ModContent.ItemType<SentientChik>());
			AddTransformation(ItemID.Katana, ModContent.ItemType<SentientKatana>());
			AddTransformation(ItemID.TragicUmbrella, ModContent.ItemType<SentientUmbrella>());

			//ranged
			AddTransformation(ItemID.BloodRainBow, ModContent.ItemType<SentientBloodRainBow>());
			AddTransformation(ItemID.Gatligator, ModContent.ItemType<SentientGatligator>());
			AddTransformation(ItemID.Toxikarp, ModContent.ItemType<SentientToxikarp>());

			//magic
			AddTransformation(ItemID.ClingerStaff, ModContent.ItemType<SentientClingerStaff>());
			AddTransformation(ItemID.SoulDrain, ModContent.ItemType<SentientLifeDrain>());

			//summon
			AddTransformation(ItemID.ImpStaff, ModContent.ItemType<SentientImpStaff>());
			AddTransformation(ItemID.HoundiusShootius, ModContent.ItemType<SentientShootius>());
			AddTransformation(ItemID.BlandWhip, ModContent.ItemType<SentientLeatherWhip>());
        }

        public override void AddRecipes()
        {
            foreach (int input in inputItems)
			{
				int output = transform[input];
                Recipe recipe = Recipe.Create(output, 1);
				recipe.AddIngredient(input, 1)
				.AddIngredient(ModContent.ItemType<SentientHeart>())
				.AddTile(ModContent.TileType<Cauldron>())
				.Register();
            }
        }

        public static void AddTransformation(int input, int output)
		{
			transform.Add(input, output);
			inputItems.Add(input);
			outputItems.Add(output);
		}

		public static void SpawnItemTransform(Player player, int inItemID, int outItemID)
		{
			Tile tile = default;
            int x = (int)player.Center.X / 16;
            int y = (int)player.Center.Y / 16;
            bool breakOut = false;
			int i = 0;
			int j = 0;
            for (i = -10; i < 10; i++)
			{
				for (j = -10; j < 10; j++)
				{
					tile = Framing.GetTileSafely(x + i, y + j);
					if (tile.HasTile && tile.TileType == ModContent.TileType<Cauldron>())
					{
						breakOut = true;
						break;
					}
				}
				if (breakOut)
					break;
			}

			if (!breakOut)
				return;

			Vector2 offset = new Vector2(32, 0);
			Vector2 pos = (new Vector2(x + i, y + j) * 16)  + offset;

			Projectile proj = Projectile.NewProjectileDirect(new EntitySource_Misc("Cauldron"), pos, Vector2.Zero, ModContent.ProjectileType<CauldronTransformProj>(), 0, 0, player.whoAmI);
			var mp = proj.ModProjectile as CauldronTransformProj;
            mp.inputTex = Terraria.GameContent.TextureAssets.Item[inItemID].Value;
            mp.outputTex = Terraria.GameContent.TextureAssets.Item[outItemID].Value;
			mp.outputItemID = outItemID;
        }
    }

	public class CauldronGItem : GlobalItem
	{
        public override void OnCreated(Item item, ItemCreationContext context)
        {
			if (item.ModItem is ICauldronOutput && context is RecipeItemCreationContext rContext)
			{
				Recipe recipe = rContext.Recipe;
				Player player = Main.LocalPlayer;
				int inputItem = recipe.requiredItem[0].type;
				CauldronSystem.SpawnItemTransform(player, inputItem, item.type);
				item.TurnToAir();
			}
        }
    }

	public class CauldronTransformProj : ModProjectile
	{
		public Texture2D inputTex;

		public Texture2D outputTex;

		public int outputItemID;

		public bool transformed = false;

		int timer = 0;

		Vector2 initialPos = Vector2.Zero;

        public override void SetDefaults()
        {
			Projectile.width = Projectile.height = 4;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.hide = true;
			Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
			initialPos = Projectile.Center;
        }

        public override void AI()
        {
			timer++;

			if (timer > 70 && timer < 200)
			{
				Projectile.scale *= 0;
			}

			if (timer < 50)
			{
				Projectile.Center = initialPos + (new Vector2(0, -10) * EaseFunction.EaseCircularOut.Ease(timer / 50f));
			}
			else if (timer < 70)
			{
				Vector2 startPos = initialPos + new Vector2(0, -10);
				Vector2 endPos = initialPos + new Vector2(0, 64);
				float progress = (timer - 50) / 20f;
                progress = EaseFunction.EaseCircularIn.Ease(progress);
                Projectile.Center = Vector2.Lerp(startPos, endPos, progress);
			}
			else if (timer > 200)
			{
				transformed = true;
                Vector2 endPos = initialPos + new Vector2(0, -10);
                Vector2 startPos = initialPos + new Vector2(0, 64);
                float progress = (timer - 200) / 20f;

				if (timer >= 220)
				{
					Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, outputItemID);
					Projectile.Kill();
					return;
				}

				progress = EaseFunction.EaseCircularOut.Ease(progress);
                Projectile.Center = Vector2.Lerp(startPos, endPos, progress);
            }
			else if (timer % 10 == 9)
			{
                SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
            }
			else if (timer == 70)
			{
                var existing = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<CauldronDummy>()).FirstOrDefault();
                if (existing != default)
                {
					(existing.ModProjectile as CauldronDummy).shakeTimer = 130;
                }

                SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
			}
			else if (timer == 200)
			{
				Projectile.scale = 1;

				SpookyPlayer.ScreenShakeAmount = 8;

				SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Projectile.Center);

                for (int j = 0; j < 16; j++)
				{
					Vector2 vel = Main.rand.NextVector2Circular(2, 4);
					vel.Y = MathF.Abs(vel.Y) * -1;
					Dust.NewDustPerfect(Projectile.Top + new Vector2(Main.rand.Next(-24, 24), 0), ModContent.DustType<CauldronBubble>(), vel, 0, Color.White, Main.rand.NextFloat(0.75f, 1.1f));
				}
			}
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D mainTex = transformed ? outputTex : inputTex;
			Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, 0f, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
			return false;
        }
    }

    public interface ICauldronOutput
	{

	}
}
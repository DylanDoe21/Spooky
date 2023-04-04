//TODO:
//Actual animation effects
//Make cauldron dummy tile

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Sentient;
using System.Collections.Generic;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Core;
using rail;

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
            AnimationFrameHeight = 72;
            AddMapEntry(new Color(0, 128, 0));
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

        public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter > 8)
			{
				frameCounter = 0;
				frame++;

				if (frame > 3)
				{
					frame = 0;
				}
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}
    }

	public class CauldronSystem : ModSystem
	{
		public static Dictionary<int, int> transform = new Dictionary<int, int>();

		public static List<int> inputItems = new List<int>();
        public static List<int> outputItems = new List<int>();

        public override void Load()
        {
			AddTransformation(ItemID.Amethyst, ModContent.ItemType<CauldronExampleOutput>());
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
			//Projectile.scale = 0.7f;
        }

        public override void AI()
        {
			timer++;

			if (timer < 50)
			{
				Projectile.Center = initialPos + (new Vector2(0, -10) * EaseFunction.EaseCircularOut.Ease(timer / 50f));
			}
			else if (timer < 70)
			{
				Vector2 startPos = initialPos + new Vector2(0, -10);
				Vector2 endPos = initialPos + new Vector2(0, 48);
				float progress = (timer - 50) / 20f;
                progress = EaseFunction.EaseCircularIn.Ease(progress);
                Projectile.Center = Vector2.Lerp(startPos, endPos, progress);
			}
			else if (timer > 200)
			{
				transformed = true;
                Vector2 endPos = initialPos + new Vector2(0, -10);
                Vector2 startPos = initialPos + new Vector2(0, 48);
                float progress = (timer - 200) / 60f;
				if (timer >= 260)
				{
					Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, outputItemID);
					Projectile.Kill();
					return;
				}
				progress = EaseFunction.EaseCircularOut.Ease(progress);
                Projectile.Center = Vector2.Lerp(startPos, endPos, progress);
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
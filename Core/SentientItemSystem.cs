using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Core
{
    public interface ICauldronOutput
	{
	}

    public class CauldronSystem : ModSystem
	{
		public static Dictionary<int, int> transform = new Dictionary<int, int>();

		public static List<int> inputItems = new List<int>();
        public static List<int> outputItems = new List<int>();

        public override void Load()
        {
			//melee
			AddTransformation(ItemID.Katana, ModContent.ItemType<SentientKatana>());
			AddTransformation(ItemID.Chik, ModContent.ItemType<SentientChik>());
			AddTransformation(ItemID.Bladetongue, ModContent.ItemType<SentientBladeTongue>());
			AddTransformation(ItemID.PaladinsHammer, ModContent.ItemType<SentientPaladinsHammer>());
			//AddTransformation(ItemID.Keybrand, ModContent.ItemType<SentientKeybrand>());

			//ranged
			AddTransformation(ItemID.BloodRainBow, ModContent.ItemType<SentientBloodRainBow>());
			AddTransformation(ItemID.Gatligator, ModContent.ItemType<SentientGatligator>());
			AddTransformation(ItemID.Toxikarp, ModContent.ItemType<SentientToxikarp>());
			//AddTransformation(ItemID.VenusMagnum, ModContent.ItemType<SentientVenusMagnum>());
			//AddTransformation(ItemID.ProximityMineLauncher, ModContent.ItemType<SentientMineLauncher>());

			//magic
			AddTransformation(ItemID.BookofSkulls, ModContent.ItemType<SentientSkullBook>());
			AddTransformation(ItemID.ClingerStaff, ModContent.ItemType<SentientClingerStaff>());
			AddTransformation(ItemID.SoulDrain, ModContent.ItemType<SentientLifeDrain>());
			//AddTransformation(ItemID.LeafBlower, ModContent.ItemType<SentientLeafBlower>());
			//AddTransformation(ItemID.MagnetSphere, ModContent.ItemType<SentientMagnetSphere>());

			//summon
			AddTransformation(ItemID.BlandWhip, ModContent.ItemType<SentientLeatherWhip>());
			AddTransformation(ItemID.ImpStaff, ModContent.ItemType<SentientImpStaff>());
			AddTransformation(ItemID.HoundiusShootius, ModContent.ItemType<SentientShootius>());
			//AddTransformation(ItemID.RavenStaff, ModContent.ItemType<SentientRavenStaff>());
			//AddTransformation(ItemID.DeadlySphereStaff, ModContent.ItemType<SentientSphereStaff>());

			//misc stuff
			AddTransformation(ItemID.BloodFishingRod, ModContent.ItemType<SentientChumCaster>());
			AddTransformation(ItemID.GreenCap, ModContent.ItemType<SentientCap>());
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
}
    
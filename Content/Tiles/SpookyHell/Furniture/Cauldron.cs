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

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            if (SentientWeaponUpgrading.IsActive)
            {
                return false;
            }

            return SentientWeaponUpgrading.HasUpgradeableItem(Main.LocalPlayer);
        }

        public override void MouseOver(int i, int j)
        {
            if (SentientWeaponUpgrading.IsActive)
            {
                return;
            }
            var player = Main.player[Main.myPlayer];
            var upgradeableItem = SentientWeaponUpgrading.GetUpgradeableItem(Main.LocalPlayer);
            if (upgradeableItem != null)
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = upgradeableItem.type;
            }
        }

        public override bool RightClick(int i, int j)
        {
            if (SentientWeaponUpgrading.IsActive)
            {
                return false;
            }

            var player = Main.player[Main.myPlayer];
            var upgradeableItem = SentientWeaponUpgrading.GetUpgradeableItem(player);
            if (upgradeableItem != null)
            {
                //remember to edit ai[1] to correspond with what item is being upgraded
                //Projectile.NewProjectile()
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, new Vector2(i * 16f, j * 16f));
                player.ConsumeItem(upgradeableItem.type, false);
            }

            return false;
        }

        public override bool AutoSelect(int i, int j, Item item)
        {
            if (item == null)
            {
                return false;
            }

            return SentientWeaponUpgrading.IsUpgradeableItem(item);
        }
    }

    public static class SentientWeaponUpgrading
    {
        public static bool IsActive => X != 0 && Y != 0;
        public static int X = 0;
        public static int Y = 0;

        //gets the top left of the cauldron
        public static Point cauldronTopLeft()
        {
            var tile = Framing.GetTileSafely(X, Y);
            return new Point(X - tile.TileFrameX / 18, Y - tile.TileFrameY / 18);
        }

        //rectagle thingie
        public static Rectangle cauldronRectangle()
        {
            var topLeft = cauldronTopLeft();
            return new Rectangle(topLeft.X, topLeft.Y, 4, 4);
        }

        //gets the item that needs to be upgraded and returns it if one is available
        public static Item GetUpgradeableItem(Player player)
        {
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (player.inventory[i].stack > 0 && IsUpgradeableItem(player.inventory[i]))
                {
                    return player.inventory[i];
                }
            }

            return null;
        }

        //checks if the player has any upgradable item on them
        public static bool HasUpgradeableItem(Player player)
        {
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (player.inventory[i].stack > 0 && IsUpgradeableItem(player.inventory[i]))
                {
                    return true;
                }
            }
			
            return false;
        }

        //upgradable items list
        public static bool IsUpgradeableItem(Item item)
        {
            return IsUpgradeableItem(item.type);
        }

        public static bool IsUpgradeableItem(int type)
        {
            return type == ModContent.ItemType<FleshAxe>();
        }
    }
}
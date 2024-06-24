using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Moco.Projectiles;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class NoseShrine : ModTile
	{
		public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 8;
			TileObjectData.newTile.Origin = new Point16(4, 7);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AnimationFrameHeight = 144;
            AddMapEntry(new Color(84, 33, 38));
			DustType = -1;
			HitSound = SoundID.Dig;
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
			if (!NPC.AnyNPCs(ModContent.NPCType<MocoSpawner>()) && !NPC.AnyNPCs(ModContent.NPCType<MocoIntro>()) && !NPC.AnyNPCs(ModContent.NPCType<Moco>()))
			{
				frame = 0;
			}
			else
			{
				frame = 1;
			}
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled  = true;
			player.cursorItemIconID = ModContent.ItemType<CottonSwab>();
			player.cursorItemIconText = "";
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}
		
		public override bool RightClick(int i, int j)
		{
			//do not allow right clicking if moco exists
			if (NPC.AnyNPCs(ModContent.NPCType<MocoSpawner>()) || NPC.AnyNPCs(ModContent.NPCType<MocoIntro>()) || NPC.AnyNPCs(ModContent.NPCType<Moco>()))
			{
				return false;
			}

			int x = i;
			int y = j;
			while (Main.tile[x, y].TileType == Type) x--;
			x++;
			while (Main.tile[x, y].TileType == Type) y--;
			y++;

			Player player = Main.LocalPlayer;
			if (player.HasItem(ModContent.ItemType<CottonSwab>())) 
			{
				SoundEngine.PlaySound(SneezeSound, player.Center);

				//TODO: add a multiplayer packer for this
				if (Main.netMode != NetmodeID.SinglePlayer) 
				{
					//ModPacket packet = Mod.GetPacket();
					//packet.Write((byte)SpookyMessageType.SpawnMoco);
					//packet.Send();
				}
				else 
				{
					NPC.NewNPC(new EntitySource_TileInteraction(Main.LocalPlayer, x * 16 + 55, y * 16 + 50), (x * 16 + 55), (y * 16 + 50), ModContent.NPCType<MocoSpawner>());
				}
			}

			return true;
		}
    }
}
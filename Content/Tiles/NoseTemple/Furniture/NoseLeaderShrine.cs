using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.NoseCult;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class NoseLeaderShrine : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 5;
			TileObjectData.newTile.Height = 8;
			TileObjectData.newTile.Origin = new Point16(3, 7);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
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

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}

		public override bool RightClick(int i, int j)
		{
			if (NPC.AnyNPCs(ModContent.NPCType<NoseCultistLeader>()) || NPC.AnyNPCs(ModContent.NPCType<MocoIdol6>()))
			{
				return false;
			}

			//TODO: add a multiplayer packet for this
			int Idol = NPC.NewNPC(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), (int)Flags.LeaderIdolPositon.X, (int)Flags.LeaderIdolPositon.Y, ModContent.NPCType<MocoIdol6>());
            Main.npc[Idol].position.X += 8;

			return true;
		}
    }
}
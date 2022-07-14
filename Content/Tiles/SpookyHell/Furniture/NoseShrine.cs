using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Moco;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class NoseShrine : ModTile
	{
		public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/MocoSneeze1", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(3, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AnimationFrameHeight = 72;
            AddMapEntry(new Color(140, 99, 201));
			DustType = DustID.PurpleCrystalShard;
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
			if (!NPC.AnyNPCs(ModContent.NPCType<Moco>()))
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
		}
		
		public override bool RightClick(int i, int j)
		{
			//do not allow right clicking if moco exists
			if (NPC.AnyNPCs(ModContent.NPCType<Moco>()))
			{
				return true;
			}

			//check if player has a cotton swab
			Player player = Main.LocalPlayer;
			if (player.HasItem(ModContent.ItemType<CottonSwab>())) 
			{
				SoundEngine.PlaySound(SneezeSound, player.Center);

				//we need to use a special packet because tiles can't net update
				if (Main.netMode == NetmodeID.MultiplayerClient) 
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.SpawnMoco);
					packet.Send();
				}
				else 
				{
					NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Moco>());
				}
			}

			return true;
		}
    }
}
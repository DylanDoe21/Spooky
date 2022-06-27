using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.Orroboro.Phase2;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class OrroboroEgg : ModTile
	{
		public bool Spawn = false;

		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.Width = 8;
			TileObjectData.newTile.Height = 8;	
			TileObjectData.newTile.Origin = new Point16(4, 7);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AnimationFrameHeight = 144;
            AddMapEntry(new Color(86, 2, 28));
			DustType = DustID.Blood;
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
			for (int k = 0; k < Main.projectile.Length; k++)
			{
				if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<OrroboroSpawn>()) 
				{
					Spawn = true;
				}
				else
				{
					Spawn = false;
				}
			}

			if (NPC.AnyNPCs(ModContent.NPCType<OrroboroHead>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
			{
				frame = 2;
			}
			else
			{
				//this probably looks dumb being here but AnimateTile is the only hook that updates constantly
				if (OrroboroSpawn.CrackTimer <= 100 || !Spawn)
				{
					frame = 0;
				}
				if (OrroboroSpawn.CrackTimer > 100 && OrroboroSpawn.CrackTimer <= 200)
				{
					frame = 1;
				}
				if (OrroboroSpawn.CrackTimer > 200)
				{
					frame = 2;
				}
			}
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.player[Main.myPlayer];
			if (player.HasItem(ModContent.ItemType<Concoction>())) 
			{
				player.cursorItemIconEnabled  = true;
				player.cursorItemIconID = ModContent.ItemType<Concoction>();
			}
			else
			{
				player.cursorItemIconEnabled  = false;
			}
		}

		public override bool RightClick(int i, int j)
		{
			if (NPC.AnyNPCs(ModContent.NPCType<OrroboroHead>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
			{
				return true;
			}

			for (int k = 0; k < Main.projectile.Length; k++)
			{
				if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<OrroboroSpawn>()) 
				{
					return true;
				}
			}

			//check if player has the concoction
			Player player = Main.player[Main.myPlayer];
			if (player.HasItem(ModContent.ItemType<Concoction>())) 
			{
				int x = i;
				int y = j;
				while (Main.tile[x, y].TileType == Type) x--;
				x++;
				while (Main.tile[x, y].TileType == Type) y--;
				y++;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					//todo: edit to make sure its in the middle of the egg
					Projectile.NewProjectile(null, x * 16f + 65f, y * 16f + 155f, 0, -1, ModContent.ProjectileType<OrroboroSpawn>(), 0, 1, Main.myPlayer, 0, 0);
				}
			}
			else
            {
				Main.NewText("You need a special substance to open the egg", 171, 64, 255);
			}

			return true;
		}
    }
}
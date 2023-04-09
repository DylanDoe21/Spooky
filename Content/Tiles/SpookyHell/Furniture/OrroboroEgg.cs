using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.EggEvent.Projectiles;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class OrroboroEgg : ModTile
	{
		public bool Spawned = false;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
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
            AddMapEntry(new Color(125, 83, 230));
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
					Spawned = true;
				}
				else
				{
					Spawned = false;
				}
			}

			if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
			{
				frame = 2;
			}
			else
			{
				//this probably looks dumb being here but AnimateTile is the only hook that updates constantly
				if (OrroboroSpawn.CrackTimer <= 100 || !Spawned)
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
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled  = true;
			player.cursorItemIconID = ModContent.ItemType<Concoction>();
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
			if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
			{
				return true;
			}

			for (int k = 0; k < Main.projectile.Length; k++)
			{
				if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<EggEventShield>()) 
				{
					return true;
				}

				if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<OrroboroSpawn>()) 
				{
					return true;
				}
			}

			//check if player has the concoction
			Player player = Main.LocalPlayer;
			if (player.HasItem(ModContent.ItemType<Concoction>())) 
			{
				int x = i;
				int y = j;
				while (Main.tile[x, y].TileType == Type) x--;
				x++;
				while (Main.tile[x, y].TileType == Type) y--;
				y++;

				if (!Flags.downedEggEvent)
				{
					Projectile.NewProjectile(null, x * 16f + 65f, y * 16f + 100f, 0, 0, ModContent.ProjectileType<EggEventShield>(), 0, 1, Main.myPlayer, 0, 0);
				}
				else
				{
					Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, x * 16 + 65, y * 16 + 155), 
					x * 16 + 65, y * 16 + 155, 0, -1, ModContent.ProjectileType<OrroboroSpawn>(), 0, 1, Main.myPlayer, 0, 0);
				}
			}
            else
            {
				Main.NewText("You need a special substance to destroy the egg", 171, 64, 255);
			}

			return true;
		}
    }
}
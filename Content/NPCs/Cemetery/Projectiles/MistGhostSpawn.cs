using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Cemetery.Furniture;

namespace Spooky.Content.NPCs.Cemetery.Projectiles
{
    public class MistGhostSpawn : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 20;
            NPC.height = 20;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
			NPC.alpha = 255;
            NPC.aiStyle = -1;
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
			//normally, spawn mist ghosts
			if (NPC.ai[2] == 0)
			{
				int[] GhostTypes = new int[] { ModContent.NPCType<MistGhost>(), ModContent.NPCType<MistGhostFaces>(), ModContent.NPCType<MistGhostWiggle>(), ModContent.NPCType<MistGhostSwirl>() };

				if (NPC.ai[0] == 0)
				{
					NPC.ai[1]++;

					if (Main.rand.NextBool(5))
					{
						int dustGore = Dust.NewDust(NPC.position + new Vector2(0, 40), NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.15f);
						Main.dust[dustGore].color = Color.OrangeRed;
						Main.dust[dustGore].velocity.X = 0;
						Main.dust[dustGore].velocity.Y = Main.rand.NextFloat(-5f, -2f);
						Main.dust[dustGore].noGravity = true;
					}

					if (NPC.ai[1] == 30 || NPC.ai[1] == 60 || NPC.ai[1] == 90)
					{
						SoundEngine.PlaySound((Main.rand.NextBool() ? SoundID.Zombie53 : SoundID.Zombie54) with { Pitch = -1f }, NPC.Center);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-35, 36), (int)NPC.Center.Y + 40, Main.rand.Next(GhostTypes));
							Main.npc[NewNPC].velocity.Y = Main.rand.Next(-5, -2);
							Main.npc[NewNPC].alpha = 255;

							if (Main.netMode == NetmodeID.Server)
							{
								NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
							}
						}
					}

					if (NPC.ai[1] >= 120)
					{
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}
				}
				else
				{
					if (!NPC.AnyNPCs(ModContent.NPCType<MistGhost>()) && !NPC.AnyNPCs(ModContent.NPCType<MistGhostFaces>()) && 
					!NPC.AnyNPCs(ModContent.NPCType<MistGhostWiggle>()) && !NPC.AnyNPCs(ModContent.NPCType<MistGhostSwirl>()))
					{
						DoStuffWhenComplete();
						NPC.active = false;
					}
				}
			}
			//during raveyards just spawn raveyard skeletons
			else
			{
				int[] SkeletonTypes = new int[] { ModContent.NPCType<PartySkeleton1>(), ModContent.NPCType<PartySkeleton2>(), ModContent.NPCType<PartySkeleton3>(), ModContent.NPCType<PartySkeleton4>(),
				ModContent.NPCType<PartySkeleton5>(), ModContent.NPCType<PartySkeleton6>(), ModContent.NPCType<PartySkeleton7>(), ModContent.NPCType<PartySkeleton8>() };

				NPC.ai[0]++;

				if (Main.rand.NextBool(5))
				{
					int dustGore = Dust.NewDust(NPC.position + new Vector2(0, 40), NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.15f);
					Main.dust[dustGore].color = Main.DiscoColor;
					Main.dust[dustGore].velocity.X = 0;
					Main.dust[dustGore].velocity.Y = Main.rand.NextFloat(-5f, -2f);
					Main.dust[dustGore].noGravity = true;
				}

				if (NPC.ai[0] == 30 || NPC.ai[0] == 60 || NPC.ai[0] == 90 || NPC.ai[0] == 120)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-35, 36), (int)NPC.Center.Y + 40, Main.rand.Next(SkeletonTypes));
						Main.npc[NewNPC].velocity.X = Main.rand.Next(-6, 7);
						Main.npc[NewNPC].velocity.Y = Main.rand.Next(-8, -4);

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
						}
					}
				}

				if (NPC.ai[0] >= 150)
				{
					DoStuffWhenComplete();
					NPC.active = false;
				}
			}
        }

		public void DoStuffWhenComplete()
		{
			if (NPC.ai[2] == 0)
			{
				DropItems();
			}

			WorldGen.KillTile((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, fail: false);

			//spawn dust
			for (int numDusts = 0; numDusts < 30; numDusts++)
			{
				int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.15f);
				Main.dust[dustGore].color = NPC.ai[2] == 1 ? Main.DiscoColor : Color.OrangeRed;
				Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
				Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
				Main.dust[dustGore].noGravity = true;
			}

			//spawn gores
			for (int numGores = 1; numGores <= 5; numGores++)
			{
				if (Main.netMode != NetmodeID.Server)
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MysteriousTombstoneGore" + numGores).Type);
				}
			}
		}

		public void DropItems()
		{
			List<int> MiscItems = new List<int>
			{
				ModContent.ItemType<CemeteryBiomeTorchItem>(), ItemID.Grenade, ItemID.Rope, ItemID.BoneArrow
			};

			List<int> Bars = new List<int>
			{
				ItemID.SilverBar, ItemID.TungstenBar, 
			};

			List<int> UncommonItem = new List<int>
			{
				ItemID.Aglet, ItemID.PortableStool, ItemID.ClimbingClaws, ItemID.Radar, ItemID.CordageGuide
			};

			//drop misc items
			List<int> ActualMiscItem = new List<int>(MiscItems);

			ActualMiscItem = ActualMiscItem.OrderBy(x => Main.rand.Next()).ToList();

			for (int numMisc = 0; numMisc < 2; numMisc++)
			{
				int ItemToChoose = Main.rand.Next(ActualMiscItem.Count);

				int DroppedItem = Item.NewItem(NPC.GetSource_Death(), NPC.Center, ActualMiscItem[ItemToChoose], Main.rand.Next(10, 21));
				Main.item[DroppedItem].velocity = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -1));

				ActualMiscItem.RemoveAt(ItemToChoose);
			}

			//bars
			if (Main.rand.NextBool(3))
			{
				int DroppedItem = Item.NewItem(NPC.GetSource_Death(), NPC.Center, Main.rand.Next(Bars), Main.rand.Next(4, 11));
				Main.item[DroppedItem].velocity = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -1));
			}

			//uncommon loot
			if (Main.rand.NextBool(5))
			{
				int DroppedItem = Item.NewItem(NPC.GetSource_Death(), NPC.Center, Main.rand.Next(UncommonItem));
				Main.item[DroppedItem].velocity = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -1));
			}

			//bloom seed
			if (Main.rand.NextBool(8))
			{
				int DroppedItem = Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<CemeterySeed>());
				Main.item[DroppedItem].velocity = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -1));
			}

			//dissolved bone
			if (Main.rand.NextBool(15))
			{
				int DroppedItem = Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<DissolvedBone>());
				Main.item[DroppedItem].velocity = new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -1));
			}
		}
	}
}
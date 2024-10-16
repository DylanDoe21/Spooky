using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistWinged : ModNPC
	{
        Vector2 SavePosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(2f, 5f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			//vector2
			writer.WriteVector2(SavePosition);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//vector2
			SavePosition = reader.ReadVector2();
		}

		public override void SetDefaults()
        {
            NPC.lifeMax = 230;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.width = 78;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 3, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.3f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistWinged"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[1] < 180)
            {
                if (NPC.frameCounter > 2)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[0]];
            
            NPC.spriteDirection = NPC.direction;

            NPC.ai[1]++;

            if (NPC.ai[1] == 5)
            {
                SavePosition = new Vector2(Parent.Center.X + Main.rand.Next(-300, 300), Parent.Center.Y - Main.rand.Next(10, 150));

                NPC.netUpdate = true;
            }

            if (NPC.ai[1] > 5 && NPC.ai[1] < 30)
            {
                Vector2 GoTo = SavePosition;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }
            else
            {
                NPC.velocity *= 0.92f;
            }

            if (NPC.ai[1] >= 180)
            {
                if (NPC.frame.Y == 7 * NPC.height && NPC.ai[2] == 0)
                {
                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 12f;

                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<NoseCultistWingedSnot>(), NPC.damage, 4.5f);

                    NPC.ai[2]++;

                    NPC.netUpdate = true;
                }
            }

            if (NPC.frame.Y >= 8 * NPC.height)
            {
                NPC.frame.Y = 0;

                NPC.ai[1] = 0;
                NPC.ai[2] = 0;

                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostMocoCondition(), ModContent.ItemType<SnotGlob>(), 3, 1, 2));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistWingedGore" + numGores).Type);
                    }
                }
            }
        }
    }
}
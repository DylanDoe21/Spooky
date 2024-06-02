using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistMage : ModNPC
	{
        public static readonly SoundStyle SneezeSound1 = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);
        public static readonly SoundStyle SneezeSound2 = new("Spooky/Content/Sounds/Moco/MocoSneeze2", SoundType.Sound);

		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 300;
            NPC.damage = 40;
            NPC.defense = 15;
            NPC.width = 54;
			NPC.height = 78;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.4f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistMage"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[0] < 420)
            {
                //idle animation
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                //casting animation
                if (NPC.frame.Y < frameHeight * 3)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }

                if (NPC.frameCounter > 15)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
			NPC.spriteDirection = NPC.direction;

            NPC.ai[0]++;

            if (NPC.ai[0] >= 420)
            {
                //play pitched up sneezing sound like a real goofy sneeze
                if (NPC.frame.Y == 6 * NPC.height && NPC.ai[0] < 550 && NPC.ai[2] == 0)
                {
                    SoundEngine.PlaySound(SneezeSound1 with { Pitch = 0.2f, Volume = 0.5f }, NPC.Center);
                    NPC.ai[2] = 1;
                }
                else
                {
                    NPC.ai[2] = 0;
                }

                //play actual sneeze sound and shoot out booger enemies
                if (NPC.ai[0] >= 570)
                {
                    SoundEngine.PlaySound(SneezeSound2, NPC.Center);

                    Main.NewText("Enemy Spawned", Color.Green);

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;

                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistMageGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistMageCloth" + numGores).Type);
                    }
                }
            }
        }
    }
}
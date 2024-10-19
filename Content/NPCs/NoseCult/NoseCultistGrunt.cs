using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
    public class NoseCultistGrunt : ModNPC  
    {
        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { Pitch = 0.4f, Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 50;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 3, 0);
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.1f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistGrunt"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter++;
            if (NPC.localAI[1] <= 0)
            {
                //walking animation
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.frameCounter > 5)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                //falling frame
                else
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
            //sneezing animation
            else
            {
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y >= 11 * NPC.height)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] == 1)
            {
                NPC.localAI[3] = Main.rand.Next(400, 540);

                NPC.netUpdate = true;
            }

            if (NPC.localAI[0] > 1 && NPC.localAI[0] >= NPC.localAI[3] && NPC.velocity.Y == 0)
            {
                NPC.localAI[1] = 1;

                NPC.netUpdate = true;
            }

            if (NPC.localAI[1] > 0)
            {
                NPC.velocity *= 0;

                NPC.localAI[2]++;
                if (NPC.localAI[2] == 8)
                {
                    SoundEngine.PlaySound(SneezeSound, NPC.Center);

                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(NPC.direction == -1 ? -10 : 10, 0), ModContent.ProjectileType<NoseCultistGruntSnot>(), NPC.damage, 4.5f);

                    NPC.netUpdate = true;
                }

                if (NPC.localAI[2] > 38)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;

                    NPC.netUpdate = true;
                }
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
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistGruntGore" + numGores).Type);
                    }
                }
            }
        }
    }
}
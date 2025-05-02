using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class TarBlobSmall : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 90;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 22;
			NPC.height = 22;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
        }

        //TODO: hide this enemy from the bestiary entry, and make this enemy count for the bestiary killcount for the actual head segments of it

        public override void AI()
        {
            if (NPC.ai[0] == 0)
            {
                if (Main.rand.NextBool())
                {
                    float rotateSpeed = Main.rand.NextBool() ? -Main.rand.NextFloat(2f, 5f) : Main.rand.NextFloat(2f, 5f);
                    
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TarBlobSmallClub>(), ai0: NPC.whoAmI, ai1: rotateSpeed, ai2: Main.rand.Next(100, 166));
                    NPC.ai[1] = 0;
                }
                else
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TarBlobSmallMouth>(), ai0: NPC.whoAmI);
                    NPC.ai[1] = 1;
                }

                NPC.ai[0]++;
            }
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();

                if (NPC.ai[1] == 0)
                {
                    BestiaryParent.SetDefaults(ModContent.NPCType<TarBlobSmallClub>());
                    Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
                }
                else
                {
                    BestiaryParent.SetDefaults(ModContent.NPCType<TarBlobSmallMouth>());
                    Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
                }

                for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DaddyLongLegsGore" + numGores).Type);
                    }
                }
            }
        }
    }
}
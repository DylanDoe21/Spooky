using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Moco.Projectiles;
using Spooky.Content.Tiles.NoseTemple.Furniture;

namespace Spooky.Content.NPCs.Boss.Moco
{
    public class MocoSpawner : ModNPC  
    {
        public bool Shake = false;

        private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle ShatterSound = new("Spooky/Content/Sounds/Moco/MocoIdolShatter", SoundType.Sound);

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 38;
			NPC.height = 50;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC.ai[0]++;

            //spawn moco intro
            if (NPC.ai[0] == 30)
            {
                int MocoSpawnOffsetX = ((int)NPC.Center.X / 16) > (Main.maxTilesX / 2) ? -1500 : 1500;

                int MocoIntro = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + MocoSpawnOffsetX, (int)NPC.Center.Y, ModContent.NPCType<MocoIntro>(), ai3: NPC.whoAmI);

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: MocoIntro);
                }
            }

            if (NPC.ai[1] > 0)
            {
                NPC.ai[2]++;

                NPC.ai[3] += 0.05f;

                if (Shake)
                {
                    NPC.rotation += NPC.ai[3] / 20;
                    if (NPC.rotation > 0.5f)
                    {
                        Shake = false;
                    }
                }
                else
                {
                    NPC.rotation -= NPC.ai[3] / 20;
                    if (NPC.rotation < -0.5f)
                    {
                        Shake = true;
                    }
                }

                if (NPC.ai[2] >= 300)
                {
                    SoundEngine.PlaySound(ShatterSound, NPC.Center);

                    //spawn dusts
                    for (int numDusts = 0; numDusts < 45; numDusts++)
                    {
                        int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, -2f, 0, default, 3f);
                        Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-16f, 16f);
                        Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-4f, 4f);
                        Main.dust[dustGore].noGravity = true;
                    }

                    NPC.active = false;
                }
            }
        }
    }
}
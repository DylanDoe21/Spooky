using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class DahliaEye : ModNPC
    {
        float distance = 0f;

        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 38;
            NPC.height = 34;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameHeight * (int)NPC.ai[2];
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            if (!Parent.active)
            {
                NPC.active = false;
            }

            /*
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = Parent.Center.X - vector.X;
            float RotateY = Parent.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            */

            NPC.ai[1] += (Parent.velocity.X * 0.5f + Parent.velocity.Y * 0.5f);
            double rad = NPC.ai[1] * (Math.PI / 180);
            NPC.position.X = Parent.Center.X - (int)(Math.Cos(rad) * 60f) - NPC.width / 2;
            NPC.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * 60f) - NPC.height / 2;
        }
    }
}
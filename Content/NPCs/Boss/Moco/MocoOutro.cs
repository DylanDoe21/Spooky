using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.Moco
{
    public class MocoOutro : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/SpookyHell/Mocling";

        Vector2 GoToPosition;
        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { Pitch = 0.7f, Volume = 0.5f };
        public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound) { Pitch = 0.45f, Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 38;
            NPC.height = 36;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/MoclingGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = 0;
            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;

            NPC.ai[0]++;

            NPC.EncourageDespawn(60);
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class ChesterClub : ModNPC
    {
        public static readonly SoundStyle BonkSound = new("Spooky/Content/Sounds/BrickBonk", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 75;
            NPC.defense = 0;
            NPC.width = 50;
            NPC.height = 52;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            if (Parent.ai[0] == 1 || Parent.ai[0] == 3)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity;
                Color newColor = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);

                for (int repeats = 0; repeats < 4; repeats++)
                {
                    Color color = newColor;
                    color = NPC.GetAlpha(color);
                    color *= 1f - fade;
                    Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + (repeats / 4 * 6f + NPC.rotation).ToRotationVector2() * (4f * fade + 2f) - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                    Main.spriteBatch.Draw(texture, afterImagePosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
                }

                Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
            }

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(BonkSound, target.Center);
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            Player player = Main.player[Parent.target];

            //kill the club if the parent does not exist
            if (!Parent.active || Parent.life <= 0 || Parent.type != ModContent.NPCType<Chester>())
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ChesterClubGore").Type);
                }

                NPC.active = false;
            }

            NPC.spriteDirection = Parent.spriteDirection;

            switch ((int)Parent.ai[0])
            {
                case 0:
                {
                    NPC.rotation = 0;
                    GoToPosition(50, 0);

                    break;
                }

                case 1:
                {
                    if (Parent.localAI[0] < 40)
                    {
                        GoToPosition(65, -50);
                    }

                    //slow down
                    if (Parent.localAI[0] == 40)
                    {
                        NPC.velocity *= 0;
                    }

                    //fling itself at the player
                    if (Parent.localAI[0] == 55)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, NPC.Center);

                        Vector2 ChargeSpeed = player.Center - NPC.Center;
                        ChargeSpeed.Normalize();
                        ChargeSpeed *= 25f;
                        NPC.velocity = ChargeSpeed;
                    }

                    if (Parent.localAI[0] > 55)
                    {
                        NPC.rotation += 0.035f * NPC.velocity.X;
                    }

                    //go back to parent
                    if (Parent.localAI[0] == 85)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, NPC.Center);

                        Vector2 ChargeSpeed = new Vector2(Parent.Center.X + 50, Parent.Center.Y) - NPC.Center;
                        ChargeSpeed.Normalize();
                        ChargeSpeed *= 25f;
                        NPC.velocity = ChargeSpeed;
                    }

                    //slow down again
                    if (Parent.localAI[0] == 115)
                    {
                        NPC.velocity *= 0;
                    }

                    break;
                }

                case 2:
                {
                    goto case 0;
                }

                case 3:
                {
                    NPC.rotation = 0;
                    GoToPosition(50, -50);

                    break;
                }
            }
        }

        public void GoToPosition(float X, float Y)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            float goToX = (Parent.Center.X + X) - NPC.Center.X;
            float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;
            
            float speed = 0.55f;

            //move up and down 
            NPC.ai[0]++;
            goToY += (float)Math.Sin(NPC.ai[0] / 30) * 15;
            
            if (NPC.velocity.X > speed)
            {
                NPC.velocity.X *= 0.95f;
            }
            if (NPC.velocity.Y > speed)
            {
                NPC.velocity.Y *= 0.95f;
            }

            if (NPC.velocity.X < goToX)
            {
                NPC.velocity.X = NPC.velocity.X + speed;
                if (NPC.velocity.X < 0f && goToX > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + speed;
                }
            }
            else if (NPC.velocity.X > goToX)
            {
                NPC.velocity.X = NPC.velocity.X - speed;
                if (NPC.velocity.X > 0f && goToX < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - speed;
                }
            }
            if (NPC.velocity.Y < goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + speed;
                    return;
                }
            }
            else if (NPC.velocity.Y > goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - speed;
                    return;
                }
            }
        }
    }
}
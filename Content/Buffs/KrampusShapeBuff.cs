using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Buffs
{
	public class KrampusShapeBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
			Player player = Main.LocalPlayer;

			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks >= 1)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff1").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks >= 2)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff2").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks >= 3)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff3").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks >= 4)
			{	
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff4").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks >= 5)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff5").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			int StatDefense = 0;
			float StatDamage = 0f;
			float StatAttackSpeed = 0f;

			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks == 1)
			{
				StatDefense = 2;
				StatDamage = 0.03f;
				StatAttackSpeed = 0.07f;
			}
		 	if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks == 2)
			{
				StatDefense = 4;
				StatDamage = 0.06f;
				StatAttackSpeed = 0.14f;
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks == 3)
			{
				StatDefense = 6;
				StatDamage = 0.09f;
				StatAttackSpeed = 0.21f;
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks == 4)
			{
				StatDefense = 8;
				StatDamage = 0.12f;
				StatAttackSpeed = 0.28f;
			}
			if (player.GetModPlayer<SpookyPlayer>().KrampusShapeBuffStacks == 5)
			{
				StatDefense = 10;
				StatDamage = 0.15f;
				StatAttackSpeed = 0.35f;
			}

			player.statDefense += StatDefense;

			player.GetDamage(DamageClass.Generic) += StatDamage;

			player.GetAttackSpeed(DamageClass.Melee) += StatAttackSpeed;
			player.GetAttackSpeed(DamageClass.Ranged) += StatAttackSpeed;
			player.GetAttackSpeed(DamageClass.Magic) += StatAttackSpeed;
			player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += StatAttackSpeed;
		}
	}
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class HunterSoulScepter : ModItem
	{
		public int SummonMode = 1;
        public string SummonDisplay = "Warrior";

		public override void SetDefaults()
		{
			Item.damage = 42;
			Item.mana = 15;       
			Item.DamageType = DamageClass.Summon;
			Item.autoReuse = true;  
			Item.noMelee = true;  
			Item.width = 30;           
			Item.height = 70;         
			Item.useTime = 32;         
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item66; 
            Item.buffType = ModContent.BuffType<OldHunterBuff>();
			Item.shoot = ModContent.ProjectileType<OldHunterMelee>();
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -15);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			return false;
		}

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            for (var j = 0; j < 10; ++j)
            {
                string text = SummonDisplay;
                Item otherItem = Main.player[Main.myPlayer].inventory[j];

                if (otherItem == Item)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position + new Vector2(-30f, 5f) * Main.inventoryScale * 0.8f,
                    Color.Turquoise, 0f, Vector2.Zero, new Vector2(Main.inventoryScale * 0.8f), -1f, Main.inventoryScale * 0.8f);
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                SummonMode++;
                if (SummonMode > 4)
                {
                    SummonMode = 1;
                }

                SoundEngine.PlaySound(SoundID.Item72, player.Center);

                switch (SummonMode)
                {
                    case 1:
                    {
                        SummonDisplay = "Warrior";
                        Item.shoot = ModContent.ProjectileType<OldHunterMelee>();
                        break;
                    }
                    case 2:
                    {
                        SummonDisplay = "Ranger";
                        Item.shoot = ModContent.ProjectileType<OldHunterRanged>();
                        break;
                    }
                    case 3:
                    {
                        SummonDisplay = "Mage";
                        Item.shoot = ModContent.ProjectileType<OldHunterMage>();
                        break;
                    }
                    case 4:
                    {
                        SummonDisplay = "Support";
                        Item.shoot = ModContent.ProjectileType<Blank>();
                        break;
                    }
                }
            }

            return true;
        }
    }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.NPCs.Hallucinations;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.BossSummon
{
    [LegacyName("ShadowCowbell")]
    public class CowBell : ModItem
    {
        public static readonly SoundStyle CowBellSound = new("Spooky/Content/Sounds/CowBell", SoundType.Sound);

        public override void SetDefaults()
        {
            Item.damage = 10;
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = CowBellSound;
            Item.buffType = ModContent.BuffType<EntityMinionBuff>();
			Item.shoot = ModContent.ProjectileType<EntityMinion>();
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<TheMan>()))
            {
                return true;
            }

            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (!Flags.encounteredMan)
            {
                int type = ModContent.NPCType<TheMan>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else 
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{
			position = Main.MouseWorld;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (Flags.encounteredMan)
            {
                player.AddBuff(Item.buffType, 2);

                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = Item.damage;
            }

			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
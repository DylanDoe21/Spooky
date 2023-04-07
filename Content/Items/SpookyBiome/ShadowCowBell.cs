using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class ShadowCowBell : ModItem
    {
        public static readonly SoundStyle CowBellSound = new("Spooky/Content/Sounds/CowBell", SoundType.Sound);

        public override void SetDefaults()
        {
            Item.damage = 12;
			Item.mana = 10;          
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 28;           
			Item.height = 32;         
			Item.useTime = 30;         
			Item.useAnimation = 30;     
			Item.useStyle = ItemUseStyleID.Swing;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = CowBellSound;     
			Item.buffType = ModContent.BuffType<EntityMinionBuff>();
			Item.shoot = ModContent.ProjectileType<EntityMinion>();
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

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ShadowClump>(), 1)
			.AddIngredient(ModContent.ItemType<CowBell>(), 1)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
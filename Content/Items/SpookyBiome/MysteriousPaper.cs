/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
	public class MysteriousPaper : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mysterious Paper");
			Tooltip.SetDefault("Throws different colored toilet paper rolls from random locations"
			+ "\nEach colored toilet paper roll will inflict a different debuff on enemies");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 28;
            Item.height = 26;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
			Item.rare = ItemRarityID.LightRed;
			Item.useStyle = 1;
			Item.value = Item.buyPrice(gold: 15);
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 12f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Naming convention; local variable is lowercase
            int[] projectiles = { ModContent.ProjectileType<MysteriousPaperProj3>(), ModContent.ProjectileType<MysteriousPaperProj1>(), ProjectileID.EmeraldBolt};

            //NewProjectile has overload that takes Vector2 pos and Vector2 vel.
            //Main.rand is UnifiedRandom, which has method Next<T>(T[] array) method. This method spits out a random element of the given array.
            //ai0 and ai1 params are default 0 params, and can be left out or ignored.
            Projectile.NewProjectile(source, position, velocity, Main.rand.Next(projectiles), damage, knockback, player.whoAmI);
            int selected = Main.rand.Next(projectiles);
            if (selected == ModContent.ProjectileType<MysteriousPaperProj1>())
            {
                
                //Projectile.NewProjectile(source, position - new Vector2(0, 300), velocity, ModContent.ProjectileType<MysteriousPaperProj1>(), damage, knockback, player.whoAmI);

                position = Main.MouseWorld - new Vector2(0f, 600f);
                for (int i = 0; i < 1; i++)
                {
                    position.Y -= 500 * i;
                    position.X += Main.rand.Next(-6, 6) * 16f;

                    Vector2 vel = Vector2.Normalize(Main.MouseWorld - position) * Item.shootSpeed;

                    Projectile.NewProjectile(source, position, vel, selected, damage, knockback, player.whoAmI);
                }
            }
            if (selected == ProjectileID.EmeraldBolt)
            {

                //Projectile.NewProjectile(source, position - new Vector2(0, 300), velocity, ModContent.ProjectileType<MysteriousPaperProj1>(), damage, knockback, player.whoAmI);
                Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY) + Main.screenPosition;
                Vector2 vel = Vector2.Normalize(Main.MouseWorld - position) * Item.shootSpeed;
                Projectile.NewProjectile(source, mouse, vel, selected, damage, knockback, player.whoAmI);


            }

            return false;


            //Main.rand is UnifiedRandom, which has method Next<T>(T[] array) method. This method spits out a random element of the given array.
            //Optionally, I would also say, since you always know the contents and indexes of the array, you could use Main.rand.Next(3), and check against index. This removes the need to index the array for rand call, and finding the type of the given ModProjectile
            //int selected = Main.rand.Next(projectiles);

            if (selected == ModContent.ProjectileType<MysteriousPaperProj3>())
            {
                //Usually want to check this condition before using Main.MouseWorld. It's probably fine here, but I've never gone wrong doing so
                if (player.whoAmI == Main.myPlayer)
                {
                    //+new Vec2(0, -N) is the same as -new Vec2(0, N), but runs the additive operation instead
                    position = Main.MouseWorld + new Vector2(0, -600f);

                    //Only looping once just adds complexity
                    for (int i = 0; i < 2; i++)
                    {
                        //Taking 3rd approach
                        Vector2 spawnPos = position;

                        //This would go up exponentially if multiplied by i
                        spawnPos.Y -= 500;

                        spawnPos.X += Main.rand.Next(-6, 6) * 16f;

                        //Changed var to Vector2, while I know var works, showing types makes code much more human-legible, and probably makes compiling process faster.
                        //Vector2.SafeNormalize should be used here, first.
                        //Second, Normalize(new Vector2(0, -600)) is the same as new Vector2(0, -1)
                        //Finally, new Vector2(0, -1) * Item.shootSpeed is the same as new Vector2(0, -Item.shootSpeed)
                        Vector2 vel = new Vector2(0, -Item.shootSpeed);

                        //Keep your calls consistent, use NewProj or NewProjDirect, why both??
                        //Selected projectile type was not used, I assume it was meant to be used here. Otherwise, it can be removed.
                        //Not using passed knockback because.?
                        Projectile.NewProjectile(source, spawnPos, vel, selected, damage, knockback, player.whoAmI);
                    }
                }
            }
            //Did you *want* this to run? I assume not because you're writing your own Shoot code
            return false;
        }
    }
}
*/

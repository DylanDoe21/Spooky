using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientLeatherWhip : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
			Item.width = 56;          
			Item.height = 50;
			Item.rare = ModContent.RarityType<SentientRarity>();
			Item.value = Item.buyPrice(gold: 12);
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 0f;
        }
    }
}